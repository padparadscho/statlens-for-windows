// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Statlens.Models;
using Statlens.Services;

namespace Statlens.ViewModels;

public partial class MainViewModel : ViewModelBase, IDisposable
{
    private const string CoinGeckoAssetId = "stronghold-token";
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromSeconds(600);

    private readonly ICoinGeckoService _coinGeckoService;
    private readonly DispatcherTimer _refreshTimer;

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

    [ObservableProperty]
    public partial AssetData? CurrentAssetData { get; set; }

    [ObservableProperty]
    public partial bool IsRefreshing { get; set; }

    [ObservableProperty]
    public partial string? ErrorMessage { get; set; }

    public MainViewModel(ICoinGeckoService coinGeckoService)
    {
        _coinGeckoService = coinGeckoService;

        _refreshTimer = new DispatcherTimer
        {
            Interval = RefreshInterval,
        };
        _refreshTimer.Tick += OnRefreshTimerTick;

        _ = RefreshAssetDataAsync();
        _refreshTimer.Start();
    }

    [RelayCommand]
    private async Task RefreshAssetDataAsync()
    {
        IsRefreshing = true;

        try
        {
            var fetchedAssetData = await _coinGeckoService.GetAssetDataAsync(CoinGeckoAssetId);

            if (fetchedAssetData is null)
            {
                ErrorMessage = "Couldn't fetch data";
                return;
            }

            CurrentAssetData = fetchedAssetData;
            ErrorMessage = null;
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private async void OnRefreshTimerTick(object? sender, EventArgs eventArgs) => await RefreshAssetDataAsync();

    public void Dispose()
    {
        _refreshTimer.Tick -= OnRefreshTimerTick;
        _refreshTimer.Stop();
        GC.SuppressFinalize(this);
    }
}
