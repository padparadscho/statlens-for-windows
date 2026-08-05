// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

namespace Statlens.Models;

public sealed class SettingsData
{
    public bool ShowChange { get; set; } = true;

    public bool ShowSparkline { get; set; } = true;

    public bool ShowVolume { get; set; }

    public bool ShowHighLow { get; set; }

    public bool ShowSupply { get; set; }

    public bool ShowMarketCap { get; set; }

    public bool IsPinned { get; set; }

    public bool StartWithWindows { get; set; }

    public double? WindowPositionX { get; set; }

    public double? WindowPositionY { get; set; }
}
