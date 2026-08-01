// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using System.Net.Http;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Statlens.Services;
using Statlens.ViewModels;
using Statlens.Views;

namespace Statlens;

public partial class App : Application
{
    private MainWindow? _mainWindow;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var coinGeckoService = new CoinGeckoService(new HttpClient());

            _mainWindow = new MainWindow
            {
                DataContext = new MainViewModel(coinGeckoService),
            };

            desktop.MainWindow = _mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnTrayIconClicked(object? sender, System.EventArgs eventArgs) => ToggleMainWindowVisibility();

    private void OnToggleVisibilityClicked(object? sender, System.EventArgs eventArgs) => ToggleMainWindowVisibility();

    private void OnExitClicked(object? sender, System.EventArgs eventArgs)
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
}
