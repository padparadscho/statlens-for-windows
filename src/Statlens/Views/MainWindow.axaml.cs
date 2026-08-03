// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Platform;
using Statlens.Interop;

namespace Statlens.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Opened += OnOpened;
    }

    private void OnOpened(object? sender, System.EventArgs eventArgs)
    {
        var platformHandle = TryGetPlatformHandle();
        if (platformHandle is not null)
        {
            DwmCornerHelper.ApplyRoundedCorners(platformHandle.Handle);
        }
    }

    private void OnRootPointerPressed(object? sender, PointerPressedEventArgs pointerEventArgs)
    {
        var pointerPoint = pointerEventArgs.GetCurrentPoint(this);

        if (pointerPoint.Properties.IsLeftButtonPressed)
        {
            BeginMoveDrag(pointerEventArgs);
        }
        else if (pointerPoint.Properties.IsRightButtonPressed)
        {
            FlyoutBase.ShowAttachedFlyout(RootPanel);
        }
    }
}
