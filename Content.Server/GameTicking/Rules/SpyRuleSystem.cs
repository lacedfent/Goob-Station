// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.GameTicking.Rules.Components;
using Content.Server.Roles;
using Content.Shared.Roles.Components;

namespace Content.Server.GameTicking.Rules;

public sealed class SpyRuleSystem : GameRuleSystem<SpyRuleComponent>
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpyRoleComponent, GetBriefingEvent>(OnGetBriefing);
    }

    // Character screen briefing
    private void OnGetBriefing(Entity<SpyRoleComponent> role, ref GetBriefingEvent args)
    {
        var ent = args.Mind.Comp.OwnedEntity;

        if (ent is null)
            return;
        args.Append(Loc.GetString("spy-role-greeting"));
    }
}