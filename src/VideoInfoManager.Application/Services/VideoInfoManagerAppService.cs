using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Application.Interfaces;
using VideoInfoManager.Application.Models;
using VideoInfoManager.Domain.Enums;

namespace VideoInfoManager.Application.Services;

public class VideoInfoManagerAppService : IVideoInfoManagerAppService
{
    private readonly IVideoInfoAppService _videoInfoAppService;

    public VideoInfoManagerAppService(IVideoInfoAppService videoInfoAppService)
    {
        _videoInfoAppService = videoInfoAppService;
    }

    public string? ProcessData(string? textData, VideoInfoStatusEnum status)
    {
        string? results = null;
        try
        {
            string[]? files = null;
            if (textData != null && textData != "")
            {
                files = textData.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            }
            if (files != null)
            {
                results = _videoInfoAppService.AddManyFromStringArray(files, status);
            }
        }
        catch (Exception ex)
        {
            results = ex.Message;
        }

        return results;
    }

    public IEnumerable<VideoInfoDTO> GetManyVideoInfo(ICollection<string> videoInfoNames, VideoInfoRenameConfiguration[]? videoInfoRenameConfigurations)
    {
        var authors = GetOnlyAuthors(videoInfoNames, videoInfoRenameConfigurations).ToList();

        return _videoInfoAppService.GetManyContainsNameList(authors);
    }

    public IEnumerable<string> GetOnlyAuthors(ICollection<string> videoInfoNames, VideoInfoRenameConfiguration[]? videoInfoRenameConfigurations)
    {
        string separator = GetVideoInfoRenameConfigurationsSeparator(videoInfoRenameConfigurations);

        var authors = new List<string>();
        foreach (var item in videoInfoNames)
        {
            string name = GetNameOrEmpty(item, separator);
            if (name != "")
            {
                authors.Add(name);
            }
        }

        return authors;
    }

    public bool IsMultipleAuthor(string fullName, VideoInfoRenameConfiguration[]? videoInfoRenameConfigurations)
    {
        string separator = GetVideoInfoRenameConfigurationsSeparator(videoInfoRenameConfigurations);
        string[] authorSeparators = GetVideoInfoRenameConfigurationsAuthorSeparators(videoInfoRenameConfigurations);

        var name = SubstringOfString(fullName, 0, fullName.IndexOf(separator));
        if (authorSeparators.Any(name.Contains))
            return true;

        return false;
    }

    public string RenameVideoInfoName(string videoInfoName, VideoInfoRenameConfiguration[]? videoInfoRenameConfigurations)
    {
        if (string.IsNullOrEmpty(videoInfoName))
            return "";

        if (videoInfoRenameConfigurations is null || videoInfoRenameConfigurations.Length == 0)
            return videoInfoName;

        string result = videoInfoName;
        string leftVideoInfoName = videoInfoName;
        var parts = new Dictionary<int, string>();

        foreach (var videoInfoRenameConfiguration in videoInfoRenameConfigurations)
        {
            if (string.IsNullOrEmpty(leftVideoInfoName) == false)
            {
                string actual = leftVideoInfoName.Trim();
                try
                {
                    int firstDelimiterPosition = -1;
                    int lastDelimiterPosition = -1;
                    bool HasFirstDelimiter = IndexOfString(actual, videoInfoRenameConfiguration.FirstDelimiter, out firstDelimiterPosition);
                    bool HasLastDelimiter = IndexOfString(actual, videoInfoRenameConfiguration.LastDelimiter, out lastDelimiterPosition);
                    if (HasFirstDelimiter && HasLastDelimiter)
                    {
                        leftVideoInfoName = actual.Substring(lastDelimiterPosition + 1);
                        actual = actual.Substring(firstDelimiterPosition, lastDelimiterPosition + 1);
                    }
                    else if (HasFirstDelimiter && !HasLastDelimiter)
                    {
                        leftVideoInfoName = actual.Substring(0, firstDelimiterPosition);
                        actual = actual.Substring(firstDelimiterPosition);
                    }
                    else if (!HasFirstDelimiter && HasLastDelimiter)
                    {
                        leftVideoInfoName = actual.Substring(lastDelimiterPosition + 1);
                        actual = actual.Substring(0, lastDelimiterPosition + 1);
                    }

                    int ignoreDelimiterPosition;
                    bool HasIgnoreDelimiter = IndexOfString(actual, videoInfoRenameConfiguration.IgnoreDelimiter, out ignoreDelimiterPosition);
                    if (HasIgnoreDelimiter)
                    {
                        actual = actual.Substring(0, ignoreDelimiterPosition);
                    }

                    actual = Delete(actual, videoInfoRenameConfiguration.WordsToDelete);
                    if (actual.Equals(leftVideoInfoName.Trim()))
                    {
                        leftVideoInfoName = "";
                    }
                    else
                    {
                        actual = $"{actual.Trim()}{videoInfoRenameConfiguration.Separator}";
                    }

                    parts.Add(videoInfoRenameConfiguration.Position, actual);
                }
                catch
                {
                    return videoInfoName;
                }
            }
        }

        if (parts.Count > 0)
        {
            result = "";
            foreach (var part in parts.OrderBy(c => c.Key))
            {
                result = $"{result}{part.Value}";
            }
        }

        return result;
    }

    public string SubstringOfString(string text, int startIndex, int length = 0)
    {
        if (string.IsNullOrEmpty(text))
            return "";

        if (startIndex < 0 || startIndex >= text.Length)
            return text;

        if ((startIndex + length) > text.Length)
            return text;

        if (length < 1)
            return text.Substring(startIndex);

        return text.Substring(startIndex, length);
    }

    private string GetNameOrEmpty(string fileName, string separator)
    {
        int firstPos = IndexOf(fileName, separator);

        if (firstPos < 2)
            return fileName;

        return SubstringOfString(fileName, 0, firstPos - 1);
    }

    private int IndexOf(string text, params string[] args)
    {
        int pos = -1;
        foreach (var arg in args)
        {
            pos = text.IndexOf(arg.Trim());
            if (pos != -1)
            {
                break;
            }
        }

        return pos;
    }

    private string Delete(string text, params string[] args)
    {
        string result = text;
        foreach (var arg in args)
        {
            result = result.Replace(arg, "");
        }

        return result;
    }

    private bool IndexOfString(string text, string[] values, out int indexPosition)
    {
        indexPosition = -1;

        if (string.IsNullOrEmpty(text) || values is null || values.Length == 0)
            return false;

        foreach (string value in values)
        {
            if (value == "")
                continue;

            indexPosition = text.IndexOf(value);
            if (indexPosition != -1)
                return true;
        }

        return false;
    }

    private string GetVideoInfoRenameConfigurationsSeparator(VideoInfoRenameConfiguration[]? videoInfoRenameConfigurations)
    {
        string separator = " - ";

        if (videoInfoRenameConfigurations is null || videoInfoRenameConfigurations.Length == 0)
            return separator;

        var firstVideoInfoRenameConfiguration = videoInfoRenameConfigurations.OrderBy(c => c.Position)
                                                                             .FirstOrDefault();
        if (firstVideoInfoRenameConfiguration is not null)
            separator = firstVideoInfoRenameConfiguration.Separator;

        return separator;
    }

    private string[] GetVideoInfoRenameConfigurationsAuthorSeparators(VideoInfoRenameConfiguration[]? videoInfoRenameConfigurations)
    {
        var authorSeparators = new string[] { "," };

        if (videoInfoRenameConfigurations is null || videoInfoRenameConfigurations.Length == 0)
            return authorSeparators;

        var firstVideoInfoRenameConfiguration = videoInfoRenameConfigurations.OrderBy(c => c.Position)
                                                                             .FirstOrDefault();
        if (firstVideoInfoRenameConfiguration is not null)
            authorSeparators = firstVideoInfoRenameConfiguration.AuthorSeparators;

        return authorSeparators;
    }


}
