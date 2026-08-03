// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

namespace Statlens.Controls;

public sealed class StatCardsControl : Panel
{
    public static readonly StyledProperty<double> GapProperty =
        AvaloniaProperty.Register<StatCardsControl, double>(nameof(Gap), 6.0);

    public double Gap
    {
        get => GetValue(GapProperty);
        set => SetValue(GapProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var visibleChildren = Children.Where(child => child.IsVisible).ToArray();

        if (visibleChildren.Length == 0)
        {
            return default;
        }

        var gap = visibleChildren.Length > 1 ? Gap : 0;
        var childWidth = double.IsInfinity(availableSize.Width)
            ? double.PositiveInfinity
            : Math.Max(0, (availableSize.Width - gap) / visibleChildren.Length);

        var height = 0.0;
        foreach (var child in visibleChildren)
        {
            child.Measure(new Size(childWidth, availableSize.Height));
            height = Math.Max(height, child.DesiredSize.Height);
        }

        return new Size(availableSize.Width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var visibleChildren = Children.Where(child => child.IsVisible).ToArray();

        if (visibleChildren.Length == 0)
        {
            return finalSize;
        }

        var gap = visibleChildren.Length > 1 ? Gap : 0;
        var childWidth = Math.Max(0, (finalSize.Width - gap) / visibleChildren.Length);
        var left = 0.0;

        foreach (var child in visibleChildren)
        {
            child.Arrange(new Rect(left, 0, childWidth, finalSize.Height));
            left += childWidth + gap;
        }

        return finalSize;
    }
}
