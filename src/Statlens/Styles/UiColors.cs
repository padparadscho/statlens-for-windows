// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using Avalonia.Media;

namespace Statlens.Styles;

internal static class UiColors
{
    public static readonly Color Positive = Color.Parse("#26A269");
    public static readonly Color Negative = Color.Parse("#E01B24");
    public static readonly Color Neutral = Color.Parse("#808080");

    public static readonly IBrush PositiveBrush = new SolidColorBrush(Positive);
    public static readonly IBrush NegativeBrush = new SolidColorBrush(Negative);
    public static readonly IBrush NeutralBrush = new SolidColorBrush(Neutral);

    public static readonly IBrush PositiveBadgeBrush = new SolidColorBrush(Color.FromArgb(0x28, Positive.R, Positive.G, Positive.B));
    public static readonly IBrush NegativeBadgeBrush = new SolidColorBrush(Color.FromArgb(0x28, Negative.R, Negative.G, Negative.B));
    public static readonly IBrush NeutralBadgeBrush = new SolidColorBrush(Color.FromArgb(0x28, Neutral.R, Neutral.G, Neutral.B));

    public static readonly IBrush TrackBrush = new SolidColorBrush(Color.Parse("#33808080"));
}
