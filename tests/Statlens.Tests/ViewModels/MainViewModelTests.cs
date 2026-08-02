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
}
