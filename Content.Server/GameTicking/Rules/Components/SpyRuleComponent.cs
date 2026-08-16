// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Prototypes;

namespace Content.Server.GameTicking.Rules.Components;

/// <summary>
/// Stores data for <see cref="SpyRuleSystem"/>.
/// </summary>
[RegisterComponent, Access(typeof(SpyRuleSystem))]
public sealed partial class SpyRuleComponent : Component
{
    /// <summary>
    /// Items a spy can receive as loot when claiming a completed bounty.
    /// </summary>
    [DataField]
    public List<EntProtoId> LootPool = new();
}