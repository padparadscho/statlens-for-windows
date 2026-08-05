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

        using var mainViewModel = CreateMainViewModel(coinGeckoServiceMock.Object);
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

        using var mainViewModel = CreateMainViewModel(coinGeckoServiceMock.Object);
        await mainViewModel.RefreshAssetDataCommand.ExecuteAsync(null);

        Assert.Null(mainViewModel.CurrentAssetData);
        Assert.NotNull(mainViewModel.ErrorMessage);
    }

    [Fact]
    public void SettingsLoadedFromService_PopulateInitialState()
    {
        var settingsService = new FakeSettingsService(new SettingsData { ShowVolume = true, IsPinned = true });
        var coinGeckoServiceMock = new Mock<ICoinGeckoService>();
        coinGeckoServiceMock
            .Setup(service => service.GetAssetDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AssetData?)null);

        using var mainViewModel = new MainViewModel(coinGeckoServiceMock.Object, settingsService, new Mock<IStartupService>().Object);

        Assert.True(mainViewModel.ShowVolume);
        Assert.True(mainViewModel.IsPinned);
    }

    private static MainViewModel CreateMainViewModel(ICoinGeckoService coinGeckoService) =>
        new(coinGeckoService, new FakeSettingsService(new SettingsData()), new Mock<IStartupService>().Object);

    private sealed class FakeSettingsService(SettingsData settingsData) : ISettingsService
    {
        public SettingsData Current { get; } = settingsData;

        public void Save()
        {
        }
    }
}
