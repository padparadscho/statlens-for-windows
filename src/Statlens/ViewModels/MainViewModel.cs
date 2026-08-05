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
using Statlens.Styles;

namespace Statlens.ViewModels;

public partial class MainViewModel : ViewModelBase, IDisposable
{
    private const string CoinGeckoAssetId = "stronghold-token";
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromMinutes(10);

    private readonly ICoinGeckoService _coinGeckoService;
    private readonly ISettingsService _settingsService;
    private readonly IStartupService _startupService;
    private readonly DispatcherTimer _refreshTimer;
    private readonly bool _isLoadingSettings;

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
    public partial bool IsStartWithWindowsEnabled { get; set; }

    [ObservableProperty]
    public partial AssetData? CurrentAssetData { get; set; }

    [ObservableProperty]
    public partial bool IsRefreshing { get; set; }

    [ObservableProperty]
    public partial string? ErrorMessage { get; set; }

    public double? WindowPositionX { get; set; }

    public double? WindowPositionY { get; set; }

    public bool ShowStatsRow => ShowVolume || ShowMarketCap;

    public string RankText => CurrentAssetData?.MarketCapRank is { } rank ? $"#{rank}" : string.Empty;

    public bool HasRank => CurrentAssetData?.MarketCapRank is not null;

    public bool IsChangePositive => CurrentAssetData?.DailyChangePercentage >= 0;

    public IBrush ChangeBrush => CurrentAssetData?.DailyChangePercentage switch
    {
        > 0 => UiColors.PositiveBrush,
        < 0 => UiColors.NegativeBrush,
        _ => UiColors.NeutralBrush,
    };

    public IBrush ChangeBadgeBrush => CurrentAssetData?.DailyChangePercentage switch
    {
        > 0 => UiColors.PositiveBadgeBrush,
        < 0 => UiColors.NegativeBadgeBrush,
        _ => UiColors.NeutralBadgeBrush,
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

    public double SupplyPercent
    {
        get
        {
            var assetData = CurrentAssetData;
            if (assetData?.TotalSupply is not { } totalSupply || totalSupply <= 0)
            {
                return 100;
            }

            return (double)Math.Clamp(assetData.CirculatingSupply / totalSupply * 100, 0, 100);
        }
    }

    public string FormattedCirculatingSupply => CurrentAssetData is { } assetData
        ? FormatCount(assetData.CirculatingSupply)
        : string.Empty;

    public string FormattedTotalSupply => CurrentAssetData?.TotalSupply is { } totalSupply
        ? FormatCount(totalSupply)
        : "—";

    public string FormattedVolume => CurrentAssetData is { } assetData
        ? FormatLargeNumber(assetData.DailyVolume)
        : string.Empty;

    public string FormattedMarketCap => CurrentAssetData is { } assetData
        ? FormatLargeNumber(assetData.MarketCap)
        : string.Empty;

    public MainViewModel(ICoinGeckoService coinGeckoService, ISettingsService settingsService, IStartupService startupService)
    {
        _coinGeckoService = coinGeckoService;
        _settingsService = settingsService;
        _startupService = startupService;

        _isLoadingSettings = true;
        LoadSettings();
        _isLoadingSettings = false;

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
            OnPropertyChanged(nameof(SupplyPercent));
            OnPropertyChanged(nameof(FormattedCirculatingSupply));
            OnPropertyChanged(nameof(FormattedTotalSupply));
            OnPropertyChanged(nameof(FormattedVolume));
            OnPropertyChanged(nameof(FormattedMarketCap));
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

    public void SaveWindowPosition(double positionX, double positionY)
    {
        WindowPositionX = positionX;
        WindowPositionY = positionY;
        SaveSettings();
    }

    partial void OnShowVolumeChanged(bool value)
    {
        OnPropertyChanged(nameof(ShowStatsRow));
        SaveSettings();
    }

    partial void OnShowMarketCapChanged(bool value)
    {
        OnPropertyChanged(nameof(ShowStatsRow));
        SaveSettings();
    }

    partial void OnShowChangeChanged(bool value) => SaveSettings();

    partial void OnShowSparklineChanged(bool value) => SaveSettings();

    partial void OnShowHighLowChanged(bool value) => SaveSettings();

    partial void OnShowSupplyChanged(bool value) => SaveSettings();

    partial void OnIsPinnedChanged(bool value) => SaveSettings();

    partial void OnIsStartWithWindowsEnabledChanged(bool value)
    {
        _startupService.SetEnabled(value);
        SaveSettings();
    }

    private void LoadSettings()
    {
        var settingsData = _settingsService.Current;

        ShowChange = settingsData.ShowChange;
        ShowSparkline = settingsData.ShowSparkline;
        ShowVolume = settingsData.ShowVolume;
        ShowHighLow = settingsData.ShowHighLow;
        ShowSupply = settingsData.ShowSupply;
        ShowMarketCap = settingsData.ShowMarketCap;
        IsPinned = settingsData.IsPinned;
        IsStartWithWindowsEnabled = settingsData.StartWithWindows;
        WindowPositionX = settingsData.WindowPositionX;
        WindowPositionY = settingsData.WindowPositionY;
    }

    private void SaveSettings()
    {
        if (_isLoadingSettings)
        {
            return;
        }

        var settingsData = _settingsService.Current;

        settingsData.ShowChange = ShowChange;
        settingsData.ShowSparkline = ShowSparkline;
        settingsData.ShowVolume = ShowVolume;
        settingsData.ShowHighLow = ShowHighLow;
        settingsData.ShowSupply = ShowSupply;
        settingsData.ShowMarketCap = ShowMarketCap;
        settingsData.IsPinned = IsPinned;
        settingsData.StartWithWindows = IsStartWithWindowsEnabled;
        settingsData.WindowPositionX = WindowPositionX;
        settingsData.WindowPositionY = WindowPositionY;

        _settingsService.Save();
    }

    private async void OnRefreshTimerTick(object? sender, EventArgs eventArgs) => await RefreshAssetDataAsync();

    private static string FormatCount(decimal value) => value switch
    {
        >= 1_000_000_000m => $"{value / 1_000_000_000m:F1}B",
        >= 1_000_000m => $"{value / 1_000_000m:F1}M",
        >= 1_000m => $"{value / 1_000m:F1}K",
        _ => $"{value:N0}",
    };

    private static string FormatLargeNumber(decimal value) => value switch
    {
        >= 1_000_000_000_000m => $"${value / 1_000_000_000_000m:F2}T",
        >= 1_000_000_000m => $"${value / 1_000_000_000m:F2}B",
        >= 1_000_000m => $"${value / 1_000_000m:F2}M",
        >= 1_000m => $"${value / 1_000m:F2}K",
        _ => $"${value:F2}",
    };

    public void Dispose()
    {
        _refreshTimer.Tick -= OnRefreshTimerTick;
        _refreshTimer.Stop();
        GC.SuppressFinalize(this);
    }
}
