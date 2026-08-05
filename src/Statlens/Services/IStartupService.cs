// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

namespace Statlens.Services;

public interface IStartupService
{
    bool IsEnabled();

    void SetEnabled(bool enabled);
}
