// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Statlens.Controls;

public sealed class SparklineControl : Control
{
    public static readonly StyledProperty<IReadOnlyList<decimal>?> PricesProperty =
        AvaloniaProperty.Register<SparklineControl, IReadOnlyList<decimal>?>(nameof(Prices));

    public static readonly StyledProperty<IBrush> StrokeBrushProperty =
        AvaloniaProperty.Register<SparklineControl, IBrush>(nameof(StrokeBrush), Brushes.DeepSkyBlue);

    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<SparklineControl, double>(nameof(StrokeThickness), 1.5);

    static SparklineControl()
    {
        AffectsRender<SparklineControl>(PricesProperty, StrokeBrushProperty, StrokeThicknessProperty);
    }

    public IReadOnlyList<decimal>? Prices
    {
        get => GetValue(PricesProperty);
        set => SetValue(PricesProperty, value);
    }

    public IBrush StrokeBrush
    {
        get => GetValue(StrokeBrushProperty);
        set => SetValue(StrokeBrushProperty, value);
    }

    public double StrokeThickness
    {
        get => GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var prices = Prices;
        if (prices is null || prices.Count < 2)
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

        var pointCount = prices.Count;
        var horizontalStep = Bounds.Width / (pointCount - 1);

        var sparklineGeometry = new StreamGeometry();
        using (var geometryContext = sparklineGeometry.Open())
        {
            for (var pointIndex = 0; pointIndex < pointCount; pointIndex++)
            {
                var normalizedHeight = (double)((prices[pointIndex] - lowestPrice) / priceRange);
                var pointX = horizontalStep * pointIndex;
                var pointY = Bounds.Height - (normalizedHeight * Bounds.Height);

                if (pointIndex == 0)
                {
                    geometryContext.BeginFigure(new Point(pointX, pointY), isFilled: false);
                }
                else
                {
                    geometryContext.LineTo(new Point(pointX, pointY));
                }
            }

            geometryContext.EndFigure(isClosed: false);
        }

        context.DrawGeometry(null, new Pen(StrokeBrush, StrokeThickness), sparklineGeometry);
    }
}
