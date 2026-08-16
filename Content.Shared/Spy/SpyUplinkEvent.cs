// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Serialization;

namespace Content.Shared.Spy;

[Serializable, NetSerializable]
public enum SpyUplinkUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class SpyUplinkBuiState : BoundUserInterfaceState
{
    public readonly List<SpyBountyEntry> Bounties;

    public SpyUplinkBuiState(List<SpyBountyEntry> bounties)
    {
        Bounties = bounties;
    }
}

[Serializable, NetSerializable]
public sealed class SpyBountyEntry
{
    public readonly NetEntity Objective;
    public readonly string Title;
    public readonly string Description;
    public readonly float Progress;
    public readonly bool Completed;
    public readonly bool Claimed;

    public SpyBountyEntry(NetEntity objective, string title, string description, float progress, bool completed, bool claimed)
    {
        Objective = objective;
        Title = title;
        Description = description;
        Progress = progress;
        Completed = completed;
        Claimed = claimed;
    }
}

[Serializable, NetSerializable]
public sealed class SpyUplinkClaimBountyMessage : BoundUserInterfaceMessage
{
    public readonly NetEntity Objective;

    public SpyUplinkClaimBountyMessage(NetEntity objective)
    {
        Objective = objective;
    }
}