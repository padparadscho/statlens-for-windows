// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using System.Runtime.InteropServices;

namespace Statlens.Interop;

internal static partial class DwmCornerHelper
{
    private const int DwmWindowCornerPreferenceAttribute = 33;
    private const int DwmWindowCornerPreferenceRound = 2;

    [LibraryImport("dwmapi.dll")]
    private static partial int DwmSetWindowAttribute(nint windowHandle, int attribute, ref int value, int valueSize);

    public static void ApplyRoundedCorners(nint windowHandle)
    {
        var cornerPreference = DwmWindowCornerPreferenceRound;
        DwmSetWindowAttribute(windowHandle, DwmWindowCornerPreferenceAttribute, ref cornerPreference, sizeof(int));
    }
}
