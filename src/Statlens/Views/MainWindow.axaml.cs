// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using System;
using Avalonia;
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
        Deactivated += OnDeactivated;
    }

    private void OnOpened(object? sender, EventArgs eventArgs)
    {
        var platformHandle = TryGetPlatformHandle();
        if (platformHandle is not null)
        {
            DwmCornerHelper.ApplyRoundedCorners(platformHandle.Handle);
        }

        ConstrainPositionToWorkingArea();
    }

    private void OnDeactivated(object? sender, EventArgs eventArgs) => ConstrainPositionToWorkingArea();

    private void ConstrainPositionToWorkingArea()
    {
        var screen = Screens.ScreenFromWindow(this) ?? Screens.Primary;
        if (screen is null)
        {
            return;
        }

        var workingArea = screen.WorkingArea;
        var windowWidth = (int)(Width * DesktopScaling);
        var windowHeight = (int)(Height * DesktopScaling);

        var maximumX = workingArea.X + workingArea.Width - windowWidth;
        var maximumY = workingArea.Y + workingArea.Height - windowHeight;

        var constrainedX = Math.Clamp(Position.X, workingArea.X, Math.Max(workingArea.X, maximumX));
        var constrainedY = Math.Clamp(Position.Y, workingArea.Y, Math.Max(workingArea.Y, maximumY));

        if (constrainedX != Position.X || constrainedY != Position.Y)
        {
            Position = new PixelPoint(constrainedX, constrainedY);
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
