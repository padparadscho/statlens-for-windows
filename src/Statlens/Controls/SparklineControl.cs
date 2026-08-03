// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Statlens.Styles;

namespace Statlens.Controls;

public sealed class SparklineControl : Control
{
    public static readonly StyledProperty<IReadOnlyList<decimal>?> PricesProperty =
        AvaloniaProperty.Register<SparklineControl, IReadOnlyList<decimal>?>(nameof(Prices));

    public static readonly StyledProperty<bool> IsPositiveProperty =
        AvaloniaProperty.Register<SparklineControl, bool>(nameof(IsPositive), true);

    static SparklineControl()
    {
        AffectsRender<SparklineControl>(PricesProperty, IsPositiveProperty);
    }

    public IReadOnlyList<decimal>? Prices
    {
        get => GetValue(PricesProperty);
        set => SetValue(PricesProperty, value);
    }

    public bool IsPositive
    {
        get => GetValue(IsPositiveProperty);
        set => SetValue(IsPositiveProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var prices = Prices;
        if (prices is null || prices.Count < 2 || Bounds.Width <= 0 || Bounds.Height <= 0)
        {
            return;
        }

        var lowestPrice = prices[0];
        var highestPrice = prices[0];

        foreach (var price in prices)
        {
            if (price < lowestPrice)
            {
                lowestPrice = price;
            }

            if (price > highestPrice)
            {
                highestPrice = price;
            }
        }

        var priceRange = highestPrice - lowestPrice;
        if (priceRange == 0)
        {
            priceRange = 1;
        }

        var trendColor = IsPositive ? UiColors.Positive : UiColors.Negative;
        var pointCount = prices.Count;
        var horizontalStep = Bounds.Width / (pointCount - 1);
        var points = new Point[pointCount];

        for (var pointIndex = 0; pointIndex < pointCount; pointIndex++)
        {
            var normalizedHeight = (double)((prices[pointIndex] - lowestPrice) / priceRange);
            var pointX = horizontalStep * pointIndex;
            var pointY = Bounds.Height - (normalizedHeight * Bounds.Height);
            points[pointIndex] = new Point(pointX, pointY);
        }

        var areaGeometry = new StreamGeometry();
        using (var geometryContext = areaGeometry.Open())
        {
            geometryContext.BeginFigure(new Point(0, Bounds.Height), isFilled: true);
            geometryContext.LineTo(points[0]);

            foreach (var point in points)
            {
                geometryContext.LineTo(point);
            }

            geometryContext.LineTo(new Point(Bounds.Width, Bounds.Height));
            geometryContext.EndFigure(isClosed: true);
        }

        var fillBrush = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.FromArgb(0x70, trendColor.R, trendColor.G, trendColor.B), 0.0),
                new GradientStop(Color.FromArgb(0x08, trendColor.R, trendColor.G, trendColor.B), 1.0),
            },
        };

        context.DrawGeometry(fillBrush, null, areaGeometry);

        var lineGeometry = new StreamGeometry();
        using (var geometryContext = lineGeometry.Open())
        {
            geometryContext.BeginFigure(points[0], isFilled: false);

            for (var pointIndex = 1; pointIndex < points.Length; pointIndex++)
            {
                geometryContext.LineTo(points[pointIndex]);
            }

            geometryContext.EndFigure(isClosed: false);
        }

        var linePen = new Pen(new SolidColorBrush(trendColor), 1.8)
        {
            LineCap = PenLineCap.Round,
            LineJoin = PenLineJoin.Round,
        };

        context.DrawGeometry(null, linePen, lineGeometry);
    }
}
