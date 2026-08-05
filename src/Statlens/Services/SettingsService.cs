// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using System.IO;
using System.Text.Json;
using Statlens.Models;

namespace Statlens.Services;

public sealed class SettingsService : ISettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private readonly string _settingsFilePath;

    public SettingsData Current { get; private set; }

    public SettingsService()
    {
        _settingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Statlens",
            "settings.json");

        Current = Load();
    }

    public void Save()
    {
        try
        {
            var settingsDirectory = Path.GetDirectoryName(_settingsFilePath);
            if (settingsDirectory is not null)
            {
                Directory.CreateDirectory(settingsDirectory);
            }

            var settingsJson = JsonSerializer.Serialize(Current, JsonOptions);
            File.WriteAllText(_settingsFilePath, settingsJson);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private SettingsData Load()
    {
        try
        {
            if (File.Exists(_settingsFilePath))
            {
                var settingsJson = File.ReadAllText(_settingsFilePath);
                var loadedSettingsData = JsonSerializer.Deserialize<SettingsData>(settingsJson);

                if (loadedSettingsData is not null)
                {
                    return loadedSettingsData;
                }
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (JsonException)
        {
        }

        return new SettingsData();
    }
}
