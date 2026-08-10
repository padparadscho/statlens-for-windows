// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Statlens.Services;

internal static class TrayIconService
{
    private const int RenderSize = 64;

    public static WindowIcon RenderGlyphIcon(string glyph, IBrush foregroundBrush)
    {
        var formattedText = new FormattedText(
            glyph,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface("Segoe Fluent Icons"),
            RenderSize,
            foregroundBrush);

        var renderTargetBitmap = new RenderTargetBitmap(new PixelSize(RenderSize, RenderSize), new Vector(96, 96));

        using (var drawingContext = renderTargetBitmap.CreateDrawingContext())
        {
            var textOrigin = new Point(
                (RenderSize - formattedText.Width) / 2,
                (RenderSize - formattedText.Height) / 2);
            drawingContext.DrawText(formattedText, textOrigin);
        }

        return new WindowIcon(renderTargetBitmap);
    }
}
