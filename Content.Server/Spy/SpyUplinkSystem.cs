// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.GameTicking.Rules.Components;
using Content.Server.Objectives.Components;
using Content.Server.Popups;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Mind;
using Content.Shared.Objectives.Systems;
using Content.Shared.Spy;
using Content.Shared.Spy.Components;
using Content.Shared.UserInterface;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Spy;

/// <summary>
/// Handles the spy uplink device: shows a spy their current bounties
/// and pays out a random reward for each completed, claimable bounty.
/// </summary>
public sealed class SpyUplinkSystem : EntitySystem
{
    [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly SharedMindSystem _mindSystem = default!;
    [Dependency] private readonly SharedObjectivesSystem _objectivesSystem = default!;
    [Dependency] private readonly SharedHandsSystem _handsSystem = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpyUplinkComponent, BeforeActivatableUIOpenEvent>(OnBeforeOpen);
        SubscribeLocalEvent<SpyUplinkComponent, SpyUplinkClaimBountyMessage>(OnClaim);
        SubscribeLocalEvent<ObjectiveAddedEvent>(OnObjectiveAdded);
    }

    private void OnBeforeOpen(EntityUid uid, SpyUplinkComponent component, BeforeActivatableUIOpenEvent args)
    {
        component.Mind = _mindSystem.TryGetMind(args.User, out var mindId, out _) ? mindId : null;
        Log.Debug($"Spy uplink {ToPrettyString(uid)} was activated by {ToPrettyString(args.User)}, bound mind is {component.Mind}");
        UpdateUserInterface(uid, component);
    }

    private void OnClaim(EntityUid uid, SpyUplinkComponent component, SpyUplinkClaimBountyMessage args)
    {
        var user = args.Actor;

        if (component.Mind is not { } mindId
            || !TryComp<MindComponent>(mindId, out var mind)
            || !_mindSystem.TryGetMind(user, out var userMindId, out _)
            || userMindId != mindId)
        {
            _popupSystem.PopupEntity(Loc.GetString("spy-uplink-not-owner"), uid, user);
            UpdateUserInterface(uid, component);
            return;
        }

        var objective = GetEntity(args.Objective);
        if (!mind.Objectives.Contains(objective))
            return;

        var stealGroup = TryComp<StealConditionComponent>(objective, out var steal)
            ? steal.StealGroup.Id
            : null;

        if (component.Claimed.Contains(objective)
            || (stealGroup != null && IsStealGroupClaimed(stealGroup)))
        {
            _popupSystem.PopupEntity(
                Loc.GetString(stealGroup != null && !component.Claimed.Contains(objective)
                    ? "spy-uplink-claimed-by-other"
                    : "spy-uplink-already-claimed"), uid, user);
            UpdateUserInterface(uid, component);
            return;
        }

        if (!_objectivesSystem.IsCompleted(objective, (mindId, mind)))
        {
            _popupSystem.PopupEntity(Loc.GetString("spy-uplink-not-completed"), uid, user);
            return;
        }

        var difficulty = TryComp<SpyBountyComponent>(objective, out var bounty)
            ? bounty.Difficulty
            : SpyBountyDifficulty.Medium;
        var loot = PickLoot(difficulty);
        var spawned = Spawn(loot, Transform(uid).Coordinates);
        _handsSystem.PickupOrDrop(user, spawned, checkActionBlocker: false);

        component.Claimed.Add(objective);
        if (stealGroup != null)
            MarkStealGroupClaimed(stealGroup);
        _popupSystem.PopupEntity(Loc.GetString("spy-uplink-claimed"), uid, user);
        UpdateUserInterface(uid, component);
    }

    private bool IsStealGroupClaimed(string stealGroup)
    {
        var query = EntityQueryEnumerator<SpyRuleComponent>();
        while (query.MoveNext(out _, out var rule))
        {
            if (rule.ClaimedStealGroups.Contains(stealGroup))
                return true;
        }

        return false;
    }

    private void MarkStealGroupClaimed(string stealGroup)
    {
        var query = EntityQueryEnumerator<SpyRuleComponent>();
        while (query.MoveNext(out _, out var rule))
            rule.ClaimedStealGroups.Add(stealGroup);
    }

    private EntProtoId PickLoot(SpyBountyDifficulty difficulty)
    {
        var query = EntityQueryEnumerator<SpyRuleComponent>();
        while (query.MoveNext(out _, out var rule))
        {
            var pool = difficulty switch
            {
                SpyBountyDifficulty.Easy => rule.LootPoolEasy,
                SpyBountyDifficulty.Hard => rule.LootPoolHard,
                _ => rule.LootPoolMedium,
            };

            if (pool.Count > 0)
                return _random.Pick(pool);

            if (rule.LootPoolMedium.Count > 0)
                return _random.Pick(rule.LootPoolMedium);
            if (rule.LootPoolHard.Count > 0)
                return _random.Pick(rule.LootPoolHard);
            if (rule.LootPoolEasy.Count > 0)
                return _random.Pick(rule.LootPoolEasy);
        }

        return "Telecrystal5";
    }

    private void OnObjectiveAdded(ObjectiveAddedEvent args)
    {
        var query = EntityQueryEnumerator<SpyUplinkComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.Mind is not { } mindId || !TryComp<MindComponent>(mindId, out var mind))
                continue;

            if (mind.Objectives.Contains(args.Objective))
                UpdateUserInterface(uid, comp);
        }
    }

    private void UpdateUserInterface(EntityUid uid, SpyUplinkComponent component)
    {
        var bounties = new List<SpyBountyEntry>();

        if (component.Mind is { } mindId && TryComp<MindComponent>(mindId, out var mind))
        {
            foreach (var objective in mind.Objectives)
            {
                var info = _objectivesSystem.GetInfo(objective, mindId, mind);
                if (info == null)
                    continue;

                var objectiveInfo = info.Value;
                var difficulty = TryComp<SpyBountyComponent>(objective, out var bounty)
                    ? bounty.Difficulty
                    : SpyBountyDifficulty.Medium;
                bounties.Add(new SpyBountyEntry(
                    GetNetEntity(objective),
                    objectiveInfo.Title,
                    objectiveInfo.Description ?? string.Empty,
                    objectiveInfo.Progress,
                    _objectivesSystem.IsCompleted(objective, (mindId, mind)),
                    IsClaimed(objective, component),
                    difficulty));
            }
        }

        _uiSystem.SetUiState(uid, SpyUplinkUiKey.Key, new SpyUplinkBuiState(bounties));
        Log.Debug($"Sent spy uplink state for {ToPrettyString(uid)} with {bounties.Count} bounties");
    }

    private bool IsClaimed(EntityUid objective, SpyUplinkComponent component)
    {
        if (component.Claimed.Contains(objective))
            return true;

        if (!TryComp<StealConditionComponent>(objective, out var steal))
            return false;

        return IsStealGroupClaimed(steal.StealGroup.Id);
    }
}