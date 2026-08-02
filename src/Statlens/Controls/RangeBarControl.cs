// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Statlens.Controls;

public sealed class RangeBarControl : Control
{
    private static readonly IBrush TrackBrush = new SolidColorBrush(Color.Parse("#33808080"));

    public static readonly StyledProperty<double> PercentProperty =
        AvaloniaProperty.Register<RangeBarControl, double>(nameof(Percent));

    public static readonly StyledProperty<IBrush> FillBrushProperty =
        AvaloniaProperty.Register<RangeBarControl, IBrush>(nameof(FillBrush), Brushes.DodgerBlue);

    static RangeBarControl()
    {
        AffectsRender<RangeBarControl>(PercentProperty, FillBrushProperty);
    }

    public double Percent
    {
        get => GetValue(PercentProperty);
        set => SetValue(PercentProperty, value);
    }

    public IBrush FillBrush
    {
        get => GetValue(FillBrushProperty);
        set => SetValue(FillBrushProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Bounds.Width <= 0 || Bounds.Height <= 0)
        {
            return;
        }

        var cornerRadius = Bounds.Height / 2;
        var trackRect = new RoundedRect(new Rect(0, 0, Bounds.Width, Bounds.Height), cornerRadius);

        context.DrawRectangle(TrackBrush, null, trackRect);

        var clampedPercent = Math.Clamp(Percent, 0, 100);
        var fillWidth = Bounds.Width * clampedPercent / 100;

        if (fillWidth > 0.5)
        {
            var fillRect = new RoundedRect(new Rect(0, 0, fillWidth, Bounds.Height), cornerRadius);
            context.DrawRectangle(FillBrush, null, fillRect);
        }
    }
}
