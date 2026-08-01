// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using Statlens.Models;

namespace Statlens.Services;

public interface ICoinGeckoService
{
    Task<AssetData?> GetAssetDataAsync(string coinGeckoAssetId, CancellationToken cancellationToken = default);
}
