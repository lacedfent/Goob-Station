// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Shared.Spy.Components;

/// <summary>
/// How hard a spy bounty is. Determines the color shown on the
/// bounty board (green easy, yellow medium, red hard) and the
/// value of the reward paid out for claiming it.
/// </summary>
public enum SpyBountyDifficulty : byte
{
    Easy = 0,
    Medium = 1,
    Hard = 2,
}

/// <summary>
/// Marks a spy mission entity as a bounty board entry, with a difficulty tier.
/// </summary>
[RegisterComponent]
public sealed partial class SpyBountyComponent : Component
{
    [DataField]
    public SpyBountyDifficulty Difficulty = SpyBountyDifficulty.Medium;
}