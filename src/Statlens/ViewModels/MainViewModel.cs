// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using CommunityToolkit.Mvvm.ComponentModel;

namespace Statlens.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial bool IsPinned { get; set; }

    [ObservableProperty]
    public partial bool ShowChange { get; set; } = true;

    [ObservableProperty]
    public partial bool ShowSparkline { get; set; } = true;

    [ObservableProperty]
    public partial bool ShowVolume { get; set; }

    [ObservableProperty]
    public partial bool ShowHighLow { get; set; }

    [ObservableProperty]
    public partial bool ShowSupply { get; set; }

    [ObservableProperty]
    public partial bool ShowMarketCap { get; set; }
}
