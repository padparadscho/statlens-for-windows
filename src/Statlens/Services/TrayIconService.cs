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
    private const int IconSize = 16;

    public static WindowIcon RenderGlyphIcon(string glyph, IBrush foregroundBrush)
    {
        var formattedText = new FormattedText(
            glyph,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface("Segoe Fluent Icons"),
            IconSize,
            foregroundBrush);

        var renderTargetBitmap = new RenderTargetBitmap(new PixelSize(IconSize, IconSize), new Vector(96, 96));

        using (var drawingContext = renderTargetBitmap.CreateDrawingContext())
        {
            var textOrigin = new Point(
                (IconSize - formattedText.Width) / 2,
                (IconSize - formattedText.Height) / 2);
            drawingContext.DrawText(formattedText, textOrigin);
        }

        return new WindowIcon(renderTargetBitmap);
    }
}
