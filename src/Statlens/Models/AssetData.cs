// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

namespace Statlens.Models;

public sealed record AssetData
{
    public required decimal Price { get; init; }

    public decimal DailyChangePercentage { get; init; }

    public decimal DailyVolume { get; init; }

    public decimal DailyHigh { get; init; }

    public decimal DailyLow { get; init; }

    public decimal MarketCap { get; init; }

    public decimal CirculatingSupply { get; init; }

    public decimal? TotalSupply { get; init; }

    public int? MarketCapRank { get; init; }

    public IReadOnlyList<decimal> Sparkline { get; init; } = [];

    public required DateTime FetchedAt { get; init; }
}
