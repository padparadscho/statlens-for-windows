// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Statlens.Models;

namespace Statlens.Services;

public sealed class CoinGeckoService(HttpClient httpClient) : ICoinGeckoService
{
    private const string CoinGeckoBaseUrl = "https://api.coingecko.com/api/v3";
    private const int SparklinePointCount = 24;

    public async Task<AssetData?> GetAssetDataAsync(string coinGeckoAssetId, CancellationToken cancellationToken = default)
    {
        var coinGeckoRequestUrl = $"{CoinGeckoBaseUrl}/coins/markets?vs_currency=usd&ids={Uri.EscapeDataString(coinGeckoAssetId)}&price_change_percentage=24h&sparkline=true";

        try
        {
            using var httpResponse = await httpClient.GetAsync(coinGeckoRequestUrl, cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                return null;
            }

            using var responseJsonDocument = await httpResponse.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: cancellationToken);
            if (responseJsonDocument is null ||
                responseJsonDocument.RootElement.ValueKind != JsonValueKind.Array ||
                responseJsonDocument.RootElement.GetArrayLength() == 0)
            {
                return null;
            }

            var assetJsonElement = responseJsonDocument.RootElement[0];

            return new AssetData
            {
                Price = GetDecimal(assetJsonElement, "current_price"),
                DailyChangePercentage = GetDecimal(assetJsonElement, "price_change_percentage_24h"),
                DailyVolume = GetDecimal(assetJsonElement, "total_volume"),
                DailyHigh = GetDecimal(assetJsonElement, "high_24h"),
                DailyLow = GetDecimal(assetJsonElement, "low_24h"),
                MarketCap = GetDecimal(assetJsonElement, "market_cap"),
                CirculatingSupply = GetDecimal(assetJsonElement, "circulating_supply"),
                TotalSupply = TryGetDecimal(assetJsonElement, "total_supply"),
                MarketCapRank = TryGetInteger(assetJsonElement, "market_cap_rank"),
                Sparkline = ExtractSparkline(assetJsonElement),
                FetchedAt = DateTime.Now,
            };
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static List<decimal> ExtractSparkline(JsonElement assetJsonElement)
    {
        if (!assetJsonElement.TryGetProperty("sparkline_in_7d", out var sparklineJsonElement) ||
            !sparklineJsonElement.TryGetProperty("price", out var sparklinePriceArray) ||
            sparklinePriceArray.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        return [.. sparklinePriceArray.EnumerateArray()
            .Where(sparklinePriceElement => sparklinePriceElement.ValueKind == JsonValueKind.Number)
            .Select(sparklinePriceElement => sparklinePriceElement.GetDecimal())
            .TakeLast(SparklinePointCount)];
    }

    private static decimal GetDecimal(JsonElement assetJsonElement, string propertyName) => TryGetDecimal(assetJsonElement, propertyName) ?? 0m;

    private static decimal? TryGetDecimal(JsonElement assetJsonElement, string propertyName)
    {
        if (!assetJsonElement.TryGetProperty(propertyName, out var propertyValue) || propertyValue.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        return propertyValue.GetDecimal();
    }

    private static int? TryGetInteger(JsonElement assetJsonElement, string propertyName)
    {
        if (!assetJsonElement.TryGetProperty(propertyName, out var propertyValue) || propertyValue.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        return propertyValue.GetInt32();
    }
}
