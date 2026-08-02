// SPDX-FileCopyrightText: 2026 Padparadscho <contact@padparadscho.com>
// SPDX-License-Identifier: AGPL-3.0-only

using Moq;
using Statlens.Models;
using Statlens.Services;
using Statlens.ViewModels;
using Xunit;

namespace Statlens.Tests.ViewModels;

public class MainViewModelTests
{
    [Fact]
    public async Task RefreshAssetDataCommand_Executes_PopulatesCurrentAssetData()
    {
        var expectedAssetData = new AssetData
        {
            Name = "Stronghold Token",
            Symbol = "SHX",
            Price = 0.05m,
            FetchedAt = DateTime.Now,
        };

        var coinGeckoServiceMock = new Mock<ICoinGeckoService>();
        coinGeckoServiceMock
            .Setup(service => service.GetAssetDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAssetData);

        using var mainViewModel = new MainViewModel(coinGeckoServiceMock.Object);
        await mainViewModel.RefreshAssetDataCommand.ExecuteAsync(null);

        Assert.Equal(expectedAssetData, mainViewModel.CurrentAssetData);
        Assert.Null(mainViewModel.ErrorMessage);
    }

    [Fact]
    public async Task RefreshAssetDataCommand_ServiceReturnsNull_SetsErrorMessage()
    {
        var coinGeckoServiceMock = new Mock<ICoinGeckoService>();
        coinGeckoServiceMock
            .Setup(service => service.GetAssetDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AssetData?)null);

        using var mainViewModel = new MainViewModel(coinGeckoServiceMock.Object);
        await mainViewModel.RefreshAssetDataCommand.ExecuteAsync(null);

        Assert.Null(mainViewModel.CurrentAssetData);
        Assert.NotNull(mainViewModel.ErrorMessage);
    }

    [Theory]
    [InlineData(0.05, 0.04, 0.06, 50.0)]
    [InlineData(0.04, 0.04, 0.06, 0.0)]
    [InlineData(0.06, 0.04, 0.06, 100.0)]
    [InlineData(0.05, 0, 0, 100.0)]
    public async Task HighLowPercent_ComputesExpectedPosition(decimal price, decimal low, decimal high, double expectedPercent)
    {
        var assetData = new AssetData
        {
            Name = "Stronghold Token",
            Symbol = "SHX",
            Price = price,
            DailyLow = low,
            DailyHigh = high,
            FetchedAt = DateTime.Now,
        };

        var coinGeckoServiceMock = new Mock<ICoinGeckoService>();
        coinGeckoServiceMock
            .Setup(service => service.GetAssetDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(assetData);

        using var mainViewModel = new MainViewModel(coinGeckoServiceMock.Object);
        await mainViewModel.RefreshAssetDataCommand.ExecuteAsync(null);

        Assert.Equal(expectedPercent, mainViewModel.HighLowPercent, precision: 2);
    }

    [Theory]
    [InlineData(50_000_000, 100_000_000, 50.0)]
    [InlineData(100_000_000, 0, 100.0)]
    public async Task SupplyPercent_ComputesExpectedRatio(decimal circulating, decimal total, double expectedPercent)
    {
        var assetData = new AssetData
        {
            Name = "Stronghold Token",
            Symbol = "SHX",
            Price = 0.05m,
            CirculatingSupply = circulating,
            TotalSupply = total > 0 ? total : null,
            FetchedAt = DateTime.Now,
        };

        var coinGeckoServiceMock = new Mock<ICoinGeckoService>();
        coinGeckoServiceMock
            .Setup(service => service.GetAssetDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(assetData);

        using var mainViewModel = new MainViewModel(coinGeckoServiceMock.Object);
        await mainViewModel.RefreshAssetDataCommand.ExecuteAsync(null);

        Assert.Equal(expectedPercent, mainViewModel.SupplyPercent, precision: 2);
    }
}
