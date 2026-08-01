// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using System.Net;
using System.Net.Http;
using Moq;
using Moq.Protected;
using Statlens.Services;
using Xunit;

namespace Statlens.Tests.Services;

public class CoinGeckoServiceTests
{
    [Fact]
    public async Task GetAssetDataAsync_SuccessfulResponse_ReturnsParsedAssetData()
    {
        const string responseBody = """
        [
            {
                "current_price": 0.05,
                "price_change_percentage_24h": 3.2,
                "total_volume": 100000,
                "high_24h": 0.06,
                "low_24h": 0.04,
                "market_cap": 5000000,
                "circulating_supply": 100000000,
                "total_supply": 200000000,
                "market_cap_rank": 500,
                "sparkline_in_7d": { "price": [0.04, 0.045, 0.05] }
            }
        ]
        """;

        var httpClient = CreateHttpClientReturning(HttpStatusCode.OK, responseBody);
        var coinGeckoService = new CoinGeckoService(httpClient);

        var assetData = await coinGeckoService.GetAssetDataAsync("stronghold-token");

        Assert.NotNull(assetData);
        Assert.Equal(0.05m, assetData.Price);
        Assert.Equal(3.2m, assetData.DailyChangePercentage);
        Assert.Equal(500, assetData.MarketCapRank);
        Assert.Equal(3, assetData.Sparkline.Count);
    }

    [Fact]
    public async Task GetAssetDataAsync_FailedResponse_ReturnsNull()
    {
        var httpClient = CreateHttpClientReturning(HttpStatusCode.InternalServerError, string.Empty);
        var coinGeckoService = new CoinGeckoService(httpClient);

        var assetData = await coinGeckoService.GetAssetDataAsync("stronghold-token");

        Assert.Null(assetData);
    }

    [Fact]
    public async Task GetAssetDataAsync_EmptyArrayResponse_ReturnsNull()
    {
        var httpClient = CreateHttpClientReturning(HttpStatusCode.OK, "[]");
        var coinGeckoService = new CoinGeckoService(httpClient);

        var assetData = await coinGeckoService.GetAssetDataAsync("stronghold-token");

        Assert.Null(assetData);
    }

    private static HttpClient CreateHttpClientReturning(HttpStatusCode statusCode, string responseBody)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(responseBody),
            });

        return new HttpClient(handlerMock.Object);
    }
}
