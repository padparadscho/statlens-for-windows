// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Media;
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

    private static readonly IBrush PositiveBrush = new SolidColorBrush(Color.Parse("#26A269"));
    private static readonly IBrush NegativeBrush = new SolidColorBrush(Color.Parse("#E01B24"));
    private static readonly IBrush NeutralBrush = new SolidColorBrush(Color.Parse("#808080"));

    private static readonly IBrush PositiveBadgeBrush = new SolidColorBrush(Color.Parse("#2826A269"));
    private static readonly IBrush NegativeBadgeBrush = new SolidColorBrush(Color.Parse("#28E01B24"));
    private static readonly IBrush NeutralBadgeBrush = new SolidColorBrush(Color.Parse("#28808080"));

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

    public string RankText => CurrentAssetData?.MarketCapRank is { } rank ? $"#{rank}" : string.Empty;

    public bool HasRank => CurrentAssetData?.MarketCapRank is not null;

    public bool IsChangePositive => CurrentAssetData?.DailyChangePercentage >= 0;

    public IBrush ChangeBrush => CurrentAssetData?.DailyChangePercentage switch
    {
        > 0 => PositiveBrush,
        < 0 => NegativeBrush,
        _ => NeutralBrush,
    };

    public IBrush ChangeBadgeBrush => CurrentAssetData?.DailyChangePercentage switch
    {
        > 0 => PositiveBadgeBrush,
        < 0 => NegativeBadgeBrush,
        _ => NeutralBadgeBrush,
    };

    public IBrush PriceBrush => ChangeBrush;

    public double HighLowPercent
    {
        get
        {
            var assetData = CurrentAssetData;
            if (assetData is null || assetData.DailyHigh <= 0 || assetData.DailyLow <= 0 || assetData.DailyHigh <= assetData.DailyLow)
            {
                return 100;
            }

            return (double)Math.Clamp((assetData.Price - assetData.DailyLow) / (assetData.DailyHigh - assetData.DailyLow) * 100, 0, 100);
        }
    }

    public string FormattedDailyLow => CurrentAssetData is { } assetData ? $"${assetData.DailyLow:N4}" : string.Empty;

    public string FormattedDailyHigh => CurrentAssetData is { } assetData ? $"${assetData.DailyHigh:N4}" : string.Empty;

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
            OnPropertyChanged(nameof(RankText));
            OnPropertyChanged(nameof(HasRank));
            OnPropertyChanged(nameof(IsChangePositive));
            OnPropertyChanged(nameof(ChangeBrush));
            OnPropertyChanged(nameof(ChangeBadgeBrush));
            OnPropertyChanged(nameof(PriceBrush));
            OnPropertyChanged(nameof(HighLowPercent));
            OnPropertyChanged(nameof(FormattedDailyLow));
            OnPropertyChanged(nameof(FormattedDailyHigh));
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private static void OpenOnCoinGecko()
    {
        var coinGeckoPageUrl = $"https://www.coingecko.com/en/coins/{Uri.EscapeDataString(CoinGeckoAssetId)}";
        Process.Start(new ProcessStartInfo(coinGeckoPageUrl) { UseShellExecute = true });
    }

    [RelayCommand]
    private void TogglePin() => IsPinned = !IsPinned;

    private async void OnRefreshTimerTick(object? sender, EventArgs eventArgs) => await RefreshAssetDataAsync();

    public void Dispose()
    {
        _refreshTimer.Tick -= OnRefreshTimerTick;
        _refreshTimer.Stop();
        GC.SuppressFinalize(this);
    }
}
