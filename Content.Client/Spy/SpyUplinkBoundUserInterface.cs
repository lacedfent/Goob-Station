// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Spy;
using Robust.Client.UserInterface;

namespace Content.Client.Spy;

public sealed class SpyUplinkBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private SpyUplinkMenu? _window;

    public SpyUplinkBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();
        _window = this.CreateWindow<SpyUplinkMenu>();
        _window.OnClaim += OnClaim;
    }

    private void OnClaim(NetEntity objective)
    {
        SendMessage(new SpyUplinkClaimBountyMessage(objective));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (_window == null || state is not SpyUplinkBuiState cast)
            return;

        _window.UpdateState(cast);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (_window != null)
            _window.OnClaim -= OnClaim;
    }
}