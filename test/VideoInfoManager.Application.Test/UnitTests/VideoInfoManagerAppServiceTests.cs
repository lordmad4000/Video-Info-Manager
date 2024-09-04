using Moq;
using VideoInfoManager.Application.Interfaces;
using VideoInfoManager.Application.Models;
using VideoInfoManager.Application.Services;
using VideoInfoManager.Domain.Interfaces;

namespace VideoInfoManager.Application.Test.UnitTests;

public class VideoInfoManagerAppServiceTests
{
    private readonly Mock<IVideoInfoRepository> _mockVideoInfoRepository;
    private readonly IVideoInfoAppService _videoInfoAppService;
    private readonly IVideoInfoManagerAppService _videoInfoManagerAppService;

    public VideoInfoManagerAppServiceTests()
    {
        _mockVideoInfoRepository = new Mock<IVideoInfoRepository>();
        _videoInfoAppService = new VideoInfoAppService(_mockVideoInfoRepository.Object);
        _videoInfoManagerAppService = new VideoInfoManagerAppService(_videoInfoAppService);
    }

    [Theory]
    [InlineData(@"[Warner Bros Pictures / Village Roadshow Pictures] George Miller - (Furiosa: A Mad Max Saga)", "George Miller - Warner Bros Pictures - Furiosa A Mad Max Saga")]
    [InlineData(@"[Paramount Pictures.com / Lucasfilm] Steven Spielberg - (Raiders of the Lost Ark", "Steven Spielberg - Paramount Pictures - Raiders of the Lost Ark")]
    public void RenameVideoInfoName_Should_Be_Expected(string videoInfoName, string expected)
    {
        // Arrange
        var videoInfoRenameConfigurations = GetVideoInfoRenameConfigurations();

        // Act
        var result = _videoInfoManagerAppService.RenameVideoInfoName(videoInfoName, videoInfoRenameConfigurations);

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
        var result = _videoInfoManagerAppService.SubstringOfString(text, startIndex, length);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void SubstringOfString_Empty_Text_Should_Be_Empty()
    {
        // Arrange
        string text = "";

        // Act
        var result = _videoInfoManagerAppService.SubstringOfString(text, 0, 10);

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
        var result = _videoInfoManagerAppService.SubstringOfString(text, 0, 10);
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
        var videoInfoRenameConfigurations = GetVideoInfoRenameConfigurations();

        // Act
        var result = _videoInfoManagerAppService.IsMultipleAuthor(fullName, videoInfoRenameConfigurations);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetOnlyAuthors_Should_Be_Expected()
    {
        // Arrange
        var videoInfoRenameConfigurations = GetVideoInfoRenameConfigurations();

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
        var result = _videoInfoManagerAppService.GetOnlyAuthors(videoInfoNames, videoInfoRenameConfigurations)
                                                .ToList();

        // Assert
        Assert.Equal<string>(expected, result);
    }

    private VideoInfoRenameConfiguration[] GetVideoInfoRenameConfigurations()
    {
        return new VideoInfoRenameConfiguration[]
        {
            new VideoInfoRenameConfiguration
            {
                Position = 1,
                FirstDelimiter = [ "[" ],
                LastDelimiter = [ "]" ],
                IgnoreDelimiter = [ "/" ],
                WordsToDelete = [ ".com", "[", "]", ":", "(", ")", "-", ",", "/", "\\" ],
                Separator = " - "
            },
            new VideoInfoRenameConfiguration
            {
                Position = 0,
                FirstDelimiter = [ "" ],
                LastDelimiter = [ "-", "(" ],
                WordsToDelete = [ ".com", "[", "]", ":", "(", ")", "-", "/", "\\" ],
                Separator = " - ",
                AuthorSeparators = [",", "&"]
            },
            new VideoInfoRenameConfiguration
            {
                Position = 2,
                FirstDelimiter = [ "" ],
                LastDelimiter = [ "" ],
                IgnoreDelimiter = [ "/" ],
                WordsToDelete = [ ".com", "[", "]", ":", "(", ")", "-", "/", "\\" ],
            },
        };

    }

}