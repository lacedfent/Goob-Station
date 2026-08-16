// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Spy;
using Robust.Shared.Prototypes;

namespace Content.Server.GameTicking.Rules.Components;

/// <summary>
/// Stores data for <see cref="SpyRuleSystem"/>.
/// </summary>
[RegisterComponent, Access(typeof(SpyRuleSystem), typeof(SpyUplinkSystem))]
public sealed partial class SpyRuleComponent : Component
{
    /// <summary>
    /// Items a spy can receive as loot when claiming an easy bounty.
    /// </summary>
    [DataField]
    public List<EntProtoId> LootPoolEasy = new();

    /// <summary>
    /// Items a spy can receive as loot when claiming a medium bounty.
    /// </summary>
    [DataField]
    public List<EntProtoId> LootPoolMedium = new();

    /// <summary>
    /// Items a spy can receive as loot when claiming a hard bounty.
    /// </summary>
    [DataField]
    public List<EntProtoId> LootPoolHard = new();

    /// <summary>
    /// Steal groups that have already been claimed by any spy.
    /// The bounty board is shared between all spies, so only one
    /// agent can claim a given bounty.
    /// </summary>
    [ViewVariables]
    public HashSet<string> ClaimedStealGroups = new();
}