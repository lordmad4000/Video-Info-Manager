using Microsoft.Extensions.Configuration;
using Moq;
using VideoInfoManager.Application.Interfaces;
using VideoInfoManager.Application.Services;
using VideoInfoManager.Domain.Enums;
using VideoInfoManager.Domain.Interfaces;
using VideoInfoManager.Presentation.CrossCutting.Models;
using VideoInfoManager.Presentation.CrossCutting.Services;
using static System.Net.Mime.MediaTypeNames;

namespace VideoInfoManager.Application.Test.UnitTests;

public class VideoInfoManagerPresentationAppServiceTests
{
    private readonly Mock<IVideoInfoRepository> _mockVideoInfoRepository;
    private readonly IVideoInfoAppService _videoInfoAppService;
    private readonly IVideoInfoManagerPresentationAppService _videoInfoManagerPresentationAppService;
    private readonly IConfiguration _configuration;

    public VideoInfoManagerPresentationAppServiceTests()
    {
        _mockVideoInfoRepository = new Mock<IVideoInfoRepository>();
        _videoInfoAppService = new VideoInfoAppService(_mockVideoInfoRepository.Object);
        _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                                   .Build();

        _videoInfoManagerPresentationAppService = new VideoInfoManagerPresentationAppService(_videoInfoAppService, _configuration);
    }

    [Theory]
    [InlineData(@"[Warner Bros Pictures / Village Roadshow Pictures] George Miller - (Furiosa: A Mad Max Saga)", "George Miller - Warner Bros Pictures - Furiosa A Mad Max Saga")]
    [InlineData(@"[Paramount Pictures.com / Lucasfilm] Steven Spielberg - (Raiders of the Lost Ark", "Steven Spielberg - Paramount Pictures - Raiders of the Lost Ark")]
    public void RenameVideoInfoName_Should_Be_Expected(string videoInfoName, string expected)
    {
        // Arrange

        // Act
        var result = _videoInfoManagerPresentationAppService.RenameVideoInfoName(videoInfoName);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, 9, "Substring")]
    [InlineData(17, 4, "Test")]
    [InlineData(-1, 5, "SubstringOfStringTest")]
    [InlineData(21, 1, "SubstringOfStringTest")]
    [InlineData(17, 5, "SubstringOfStringTest")]
    [InlineData(17, 0, "Test")]
    [InlineData(11, 6, "String")]
    [InlineData(20, 1, "t")]
    public void SubstringOfString_Should_Be_Expected(int startIndex, int length, string expected)
    {
        // Arrange
        string text = "SubstringOfStringTest";

        // Act
        var result = _videoInfoManagerPresentationAppService.SubstringOfString(text, startIndex, length);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void SubstringOfString_Empty_Text_Should_Be_Empty()
    {
        // Arrange
        string text = "";

        // Act
        var result = _videoInfoManagerPresentationAppService.SubstringOfString(text, 0, 10);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void SubstringOfString_Null_Text_Should_Be_Empty()
    {
        // Arrange
        string? text = null;

        // Act
#pragma warning disable CS8604 // Posible argumento de referencia nulo
        var result = _videoInfoManagerPresentationAppService.SubstringOfString(text, 0, 10);
#pragma warning restore CS8604 // Posible argumento de referencia nulo

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("George Miller - Warner Bros Pictures - Furiosa A Mad Max Saga", false)]
    [InlineData("Lilly Wachowski ; Lana Wachowski - Warner Bros Pictures - The Matrix", false)]
    [InlineData("Bobby Farrell, Peter Farrelly - 20th Century Fox - Me, Myself & Irene", true)]
    [InlineData("Lilly Wachowski & Lana Wachowski - Warner Bros Pictures - The Matrix", true)]
    public void IsMultipleAuthor_Should_Be_Expected(string fullName, bool expected)
    {
        // Arrange

        // Act
        var result = _videoInfoManagerPresentationAppService.IsMultipleAuthor(fullName);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetOnlyAuthors_Should_Be_Expected()
    {
        // Arrange

        var videoInfoNames = new List<string>
        {
            "George Miller - Warner Bros Pictures - Furiosa A Mad Max Saga",
            "Bobby Farrell, Peter Farrelly - 20th Century Fox - Me, Myself & Irene",
            "Lilly Wachowski & Lana Wachowski - Warner Bros Pictures - The Matrix",
            "Steven Spielberg - Paramount Pictures - Raiders of the Lost Ark"
        };

        var expected = new List<string>
        {
            "George Miller",
            "Bobby Farrell, Peter Farrelly",
            "Lilly Wachowski & Lana Wachowski",
            "Steven Spielberg"
        };

        // Act
        var result = _videoInfoManagerPresentationAppService.GetOnlyAuthors(videoInfoNames)
                                                            .ToList();

        // Assert
        Assert.Equal<string>(expected, result);
    }

    [Fact]
    public void NormalizeFileName_Should_Be_Expected()
    {
        // Arrange
        string fileName = @"C:\Media\Videos\George Miller - Warner Bros Pictures - Furiosa A Mad Max Saga.mp40";
        string expected = "George Miller - Warner Bros Pictures - Furiosa A Mad Max Saga";

        // Act
        var result = _videoInfoManagerPresentationAppService.NormalizeFileName(fileName);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void NormalizeFileName_Null_FileName_Should_Be_Null()
    {
        // Arrange
        string? fileName = null;

        // Act
#pragma warning disable CS8604 // Posible argumento de referencia nulo
        var result = _videoInfoManagerPresentationAppService.NormalizeFileName(fileName);
#pragma warning restore CS8604 // Posible argumento de referencia nulo

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void NormalizeFileName_Empty_FileName_Should_Be_Empty()
    {
        // Arrange
        string fileName = "";

        // Act
        var result = _videoInfoManagerPresentationAppService.NormalizeFileName(fileName);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetFirstItem_Should_Be_Expected()
    {
        // Arrange
        string[] source =
        {
            "George Miller -Warner Bros Pictures - Furiosa A Mad Max Saga",
            "Lilly Wachowski, Lana Wachowski - Warner Bros Pictures - The Matrix",
            "Bobby Farrell, Peter Farrelly - 20th Century Fox - Me, Myself & Irene"
        };
        string sourceText = string.Join(Environment.NewLine, source);
        string expected = "George Miller -Warner Bros Pictures - Furiosa A Mad Max Saga";

        // Act
        var result = _videoInfoManagerPresentationAppService.GetFirstItem(sourceText);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetFirstItem_Null_SourceText_Should_Be_Null()
    {
        // Arrange
        string? sourceText = null;

        // Act
#pragma warning disable CS8604 // Posible argumento de referencia nulo
        var result = _videoInfoManagerPresentationAppService.GetFirstItem(sourceText);
#pragma warning restore CS8604 // Posible argumento de referencia nulo

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetFirstItem_Empty_SourceText_Should_Be_Empty()
    {
        // Arrange
        string sourceText = "";

        // Act
        var result = _videoInfoManagerPresentationAppService.GetFirstItem(sourceText);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void RemoveFirstItem_Should_Be_Expected()
    {
        // Arrange
        string[] source =
        {
            "George Miller -Warner Bros Pictures - Furiosa A Mad Max Saga",
            "Lilly Wachowski, Lana Wachowski - Warner Bros Pictures - The Matrix",
            "Bobby Farrell, Peter Farrelly - 20th Century Fox - Me, Myself & Irene"
        };
        string sourceText = string.Join(Environment.NewLine, source);
        string expected = @$"Lilly Wachowski, Lana Wachowski - Warner Bros Pictures - The Matrix{Environment.NewLine}Bobby Farrell, Peter Farrelly - 20th Century Fox - Me, Myself & Irene{Environment.NewLine}";

        // Act
        var result = _videoInfoManagerPresentationAppService.RemoveFirstItem(sourceText);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void RemoveFirstItem_Null_SourceText_Be_Null()
    {
        // Arrange
        string? sourceText = null;

        // Act
#pragma warning disable CS8604 // Posible argumento de referencia nulo
        var result = _videoInfoManagerPresentationAppService.RemoveFirstItem(sourceText);
#pragma warning restore CS8604 // Posible argumento de referencia nulo

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void RemoveFirstItem_Empty_SourceText_Be_Empty()
    {
        // Arrange
        string sourceText = "";

        // Act
        var result = _videoInfoManagerPresentationAppService.RemoveFirstItem(sourceText);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetVideoInfoStatusByConfigurationName_Should_Be_Expected()
    {
        // Arrange
        string configurationName = "Pended";
        VideoInfoStatus expected = new VideoInfoStatus
        {
            ConfigurationName = "Pended",
            StatusName = "Pended",
            Status = VideoInfoStatusEnum.Pended
        };

        // Act
        VideoInfoStatus result = _videoInfoManagerPresentationAppService.GetVideoInfoStatusByConfigurationName(configurationName);

        // Assert
        Assert.Equal(expected.ConfigurationName, result.ConfigurationName);
        Assert.Equal(expected.StatusName, result.StatusName);
        Assert.Equal(expected.Status, result.Status);
    }

    [Fact]
    public void GetVideoInfoStatusByConfigurationName_Null_ConfigurationName_Should_Be_Default_VideoInfoStatus()
    {
        // Arrange
        string? configurationName = null;
        var defaultVideoInfoStatus = new VideoInfoStatus();

        // Act
#pragma warning disable CS8604 // Posible argumento de referencia nulo
        VideoInfoStatus result = _videoInfoManagerPresentationAppService.GetVideoInfoStatusByConfigurationName(configurationName);
#pragma warning restore CS8604 // Posible argumento de referencia nulo

        // Assert
        Assert.Equal(defaultVideoInfoStatus.ConfigurationName, result.ConfigurationName);
        Assert.Equal(defaultVideoInfoStatus.StatusName, result.StatusName);
        Assert.Equal(defaultVideoInfoStatus.Status, result.Status);
    }

    [Fact]
    public void GetVideoInfoStatusByConfigurationName_Wrong_ConfigurationName_Should_Be_Default_VideoInfoStatus()
    {
        // Arrange
        string configurationName = "Removed";
        var defaultVideoInfoStatus = new VideoInfoStatus();

        // Act
        VideoInfoStatus result = _videoInfoManagerPresentationAppService.GetVideoInfoStatusByConfigurationName(configurationName);

        // Assert
        Assert.Equal(defaultVideoInfoStatus.ConfigurationName, result.ConfigurationName);
        Assert.Equal(defaultVideoInfoStatus.StatusName, result.StatusName);
        Assert.Equal(defaultVideoInfoStatus.Status, result.Status);
    }

    // Pruebas sin realizar aún:
    // Search,
    // LastSearch,
    // GetResults,
    // ProcessData,
    // GetManyVideoInfo,
    // Update
    // Delete,
    // GetAllDataOrderByName


}