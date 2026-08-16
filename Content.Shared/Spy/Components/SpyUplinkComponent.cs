// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared.Spy.Components;

/// <summary>
/// A small handheld device that displays a spy's current bounties
/// and allows them to claim a reward once a bounty is completed.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class SpyUplinkComponent : Component
{
    /// <summary>
    /// The mind this uplink is currently bound to. Server-side only.
    /// </summary>
    [ViewVariables]
    public EntityUid? Mind;

    /// <summary>
    /// Bounties that have already been claimed. Server-side only.
    /// </summary>
    [ViewVariables]
    public HashSet<EntityUid> Claimed = new();
}