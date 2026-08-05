// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using System.Runtime.Versioning;
using Microsoft.Win32;

namespace Statlens.Services;

[SupportedOSPlatform("windows")]
public sealed class StartupService : IStartupService
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AutorunValueName = "Statlens";

    public bool IsEnabled()
    {
        try
        {
            using var runRegistryKey = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
            return runRegistryKey?.GetValue(AutorunValueName) is not null;
        }
        catch (System.Security.SecurityException)
        {
            return false;
        }
    }

    public void SetEnabled(bool enabled)
    {
        try
        {
            using var runRegistryKey = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true)
                ?? Registry.CurrentUser.CreateSubKey(RunKeyPath);

            if (enabled)
            {
                var executablePath = Environment.ProcessPath
                    ?? throw new InvalidOperationException("Could not determine the executable path.");
                runRegistryKey.SetValue(AutorunValueName, $"\"{executablePath}\"");
            }
            else
            {
                runRegistryKey.DeleteValue(AutorunValueName, throwOnMissingValue: false);
            }
        }
        catch (System.Security.SecurityException)
        {
        }
    }
}
