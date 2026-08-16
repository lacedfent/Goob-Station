// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Server.GameTicking.Rules.Components;

/// <summary>
/// Stores data for <see cref="SpyRuleSystem"/>.
/// </summary>
[RegisterComponent, Access(typeof(SpyRuleSystem))]
public sealed partial class SpyRuleComponent : Component;