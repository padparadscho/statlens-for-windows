// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using System;
using System.ComponentModel;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using Statlens.Services;
using Statlens.ViewModels;
using Statlens.Views;

namespace Statlens;

public partial class App : Application, IDisposable
{
    private MainWindow? _mainWindow;
    private MainViewModel? _mainViewModel;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var coinGeckoService = new CoinGeckoService(new HttpClient());
            var settingsService = new SettingsService();
            var startupService = new StartupService();

            _mainViewModel = new MainViewModel(coinGeckoService, settingsService, startupService);
            _mainViewModel.PropertyChanged += OnMainViewModelPropertyChanged;

            _mainWindow = new MainWindow
            {
                DataContext = _mainViewModel,
            };

            if (_mainViewModel.WindowPositionX is { } positionX && _mainViewModel.WindowPositionY is { } positionY)
            {
                _mainWindow.Position = new PixelPoint((int)positionX, (int)positionY);
                _mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            }

            _mainWindow.PositionChanged += OnMainWindowPositionChanged;

            desktop.MainWindow = _mainWindow;
            desktop.ShutdownRequested += OnShutdownRequested;

            ActualThemeVariantChanged += OnActualThemeVariantChanged;

            UpdateTrayIcon();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnActualThemeVariantChanged(object? sender, EventArgs eventArgs) => UpdateTrayIcon();

    private void OnMainViewModelPropertyChanged(object? sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (propertyChangedEventArgs.PropertyName == nameof(MainViewModel.CurrentAssetData))
        {
            UpdateTrayIcon();
        }
    }

    private void UpdateTrayIcon()
    {
        var trayIcons = TrayIcon.GetIcons(this);
        if (trayIcons is not { Count: > 0 } || _mainViewModel is null)
        {
            return;
        }

        var glyph = _mainViewModel.IsChangePositive ? "\uEAFC" : "\uEF42";
        var iconBrush = ActualThemeVariant == ThemeVariant.Light ? Brushes.Black : Brushes.White;
        trayIcons[0].Icon = TrayIconService.RenderGlyphIcon(glyph, iconBrush);
    }

    private void OnMainWindowPositionChanged(object? sender, PixelPointEventArgs pixelPointEventArgs) =>
        _mainViewModel?.SaveWindowPosition(pixelPointEventArgs.Point.X, pixelPointEventArgs.Point.Y);

    private void OnShutdownRequested(object? sender, ShutdownRequestedEventArgs shutdownRequestedEventArgs) => Dispose();

    private void OnTrayIconClicked(object? sender, EventArgs eventArgs) => ToggleMainWindowVisibility();

    private void OnExitClicked(object? sender, EventArgs eventArgs)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    private void ToggleMainWindowVisibility()
    {
        if (_mainWindow is null)
        {
            return;
        }

        if (_mainWindow.IsVisible)
        {
            _mainWindow.Hide();
        }
        else
        {
            _mainWindow.Show();
        }
    }

    public void Dispose()
    {
        _mainViewModel?.Dispose();
        GC.SuppressFinalize(this);
    }
}
