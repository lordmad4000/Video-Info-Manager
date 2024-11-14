using Microsoft.Extensions.Configuration;
using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Application.UseCases;
using VideoInfoManager.Domain.Enums;
using VideoInfoManager.Presentation.Crosscutting.Extensions;
using VideoInfoManager.Presentation.CrossCutting.Models;

namespace VideoInfoManager.Presentation.CrossCutting.Services;


public class VideoInfoManagerPresentationAppService : IVideoInfoManagerPresentationAppService
{
    private const int MinAuthorName = 2;
    private List<VideoInfoStatus> _videoInfoStatuses = new List<VideoInfoStatus>();
    private string[]? _lastSearch { get; set; } = null;
    private IEnumerable<VideoInfoDTO>? _lastSearchData { get; set; } = null;
    private VideoInfoDTO? _videoInfoSelected { get; set; } = null;
    private List<VideoInfoDTO> _results = new List<VideoInfoDTO>();

    private readonly VideoInfoUseCases _videoInfoUseCases;
    private readonly IConfiguration _configuration;
    private readonly VideoInfoRenameConfiguration[]? _videoInfoRenameConfigurations;


    public VideoInfoManagerPresentationAppService(VideoInfoUseCases videoInfoUseCases, IConfiguration configuration)
    {
        _videoInfoUseCases = videoInfoUseCases;
        _configuration = configuration;
        _videoInfoRenameConfigurations = _configuration.GetSection("VideoInfoRenameConfiguration").Get<VideoInfoRenameConfiguration[]>();
        InitializeVideoInfoStatuses();
    }

    public List<VideoInfoStatus> GetVideoInfoStatuses() => _videoInfoStatuses;
    public List<VideoInfoDTO> GetResults() => _results;
    public void LastSearch(List<VideoInfoStatusEnum> videoInfoActiveStatuses, bool isVideoName = false) => Search(_lastSearch, videoInfoActiveStatuses, isVideoName);
    public void Search(string[]? search, List<VideoInfoStatusEnum> videoInfoActiveStatuses, bool isVideoName = false)
    {
        if (search is null || search.Count() == 0)
            return;

        _lastSearch = search;
        _lastSearchData = isVideoName == true
                     ? GetManyVideoInfo(search)
                     : _videoInfoUseCases.GetAllVideoInfoContainsNameListQueryHandler.Handle(search.ToList());

        if (_lastSearchData is not null)
        {
            var data = _lastSearchData.Where(c => videoInfoActiveStatuses.Contains(c.StatusToVideoInfoStatusEnum()))
                                      .ToList();

            var multipleAuthors = data.Where(c => IsMultipleAuthor(c.Name));
            var singleAuthor = data.Where(c => IsMultipleAuthor(c.Name) is false);
            var videoInforSearchList = new List<VideoInfoDTO>();

            if (singleAuthor.Any())
            {
                videoInforSearchList.AddRange(singleAuthor.OrderBy(c => c.Name)
                                                          .ThenByDescending(c => c.Status));
            }
            if (multipleAuthors.Any())
            {
                videoInforSearchList.AddRange(multipleAuthors.OrderBy(c => c.Name)
                                                             .ThenByDescending(c => c.Status));
            }

            _results = GetVideoInfoWithConfigurationNames(videoInforSearchList).ToList();
        }
    }

    public string? ProcessData(string? textData, string status)
    {
        string? results = null;

        try
        {
            VideoInfoStatusEnum videoInfoStatus = GetVideoInfoStatusByConfigurationName(status).Status;
            string[]? files = null;
            if (textData != null && textData != "")
            {
                files = textData.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            }
            if (files != null)
            {
                results = _videoInfoUseCases.CreateManyVideoInfoCommandHandler.Handle(files, videoInfoStatus);
            }
        }
        catch (Exception ex)
        {
            results = ex.Message;
        }

        return results;
    }

    public IEnumerable<VideoInfoDTO> GetManyVideoInfo(ICollection<string> videoInfoNames)
    {
        var authors = GetOnlyAuthors(videoInfoNames);

        return _videoInfoUseCases.GetAllVideoInfoContainsNameListQueryHandler.Handle(authors.Distinct().ToList());
    }

    public bool Update(VideoInfoDTO videoInfoDTO)
    {
        videoInfoDTO.Status = GetVideoInfoStatusByConfigurationName(videoInfoDTO.Status).StatusName;

        return _videoInfoUseCases.UpdateVideoInfoCommandHandler.Handle(videoInfoDTO);
    }

    public bool Delete(VideoInfoDTO videoInfoDTO)
    {
        if (videoInfoDTO is null)
        {
            return false;
        }

        return _videoInfoUseCases.DeleteVideoInfoCommandHandler.Handle(videoInfoDTO.Id);
    }

    public string NormalizeFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName) == false)
        {
            fileName = GetFileNameOnly(fileName);
        }

        return fileName;
    }

    public VideoInfoStatus GetVideoInfoStatusByConfigurationName(string configurationName)
    {
        VideoInfoStatus? videoinfoStatus = _videoInfoStatuses.FirstOrDefault(c => c.ConfigurationName.Equals(configurationName));
        if (videoinfoStatus is null)
            return new VideoInfoStatus();

        return videoinfoStatus;
    }

    public IEnumerable<string> GetOnlyAuthors(ICollection<string> videoInfoNames)
    {
        string separator = GetVideoInfoRenameConfigurationsSeparator(_videoInfoRenameConfigurations);

        var authors = new List<string>();
        foreach (var item in videoInfoNames)
        {
            string name = GetNameOrEmpty(item, separator);
            if (name != "")
            {
                if (name.Contains(","))
                {
                    string[]? manyAuthors = name.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (manyAuthors != null)
                    {
                        foreach (var author in manyAuthors)
                        {
                            if (author.Length > MinAuthorName)
                            {
                                authors.Add(author);
                            }
                        }
                    }
                }
                else if (name.Length > MinAuthorName)
                {
                    authors.Add(name);
                }
            }
        }

        return authors;
    }

    public bool IsMultipleAuthor(string fullName)
    {
        string separator = GetVideoInfoRenameConfigurationsSeparator(_videoInfoRenameConfigurations);
        string[] authorSeparators = GetVideoInfoRenameConfigurationsAuthorSeparators(_videoInfoRenameConfigurations);

        var name = SubstringOfString(fullName, 0, fullName.IndexOf(separator));
        if (authorSeparators.Any(name.Contains))
            return true;

        return false;
    }

    public string RenameVideoInfoName(string videoInfoName)
    {
        if (string.IsNullOrEmpty(videoInfoName))
            return "";

        if (_videoInfoRenameConfigurations is null || _videoInfoRenameConfigurations.Length == 0)
            return videoInfoName;

        string result = videoInfoName;
        string leftVideoInfoName = videoInfoName;
        var parts = new Dictionary<int, string>();

        foreach (var videoInfoRenameConfiguration in _videoInfoRenameConfigurations)
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

    public string RemoveFirstItem(string sourceText)
    {
        if (string.IsNullOrWhiteSpace(sourceText) is false)
        {
            var splitText = sourceText.SplitNewLine(StringSplitOptions.RemoveEmptyEntries);
            if (splitText.Length > 0)
            {
                sourceText = "";
                for (int i = 1; i < splitText.Length; i++)
                {
                    sourceText += $"{splitText[i].RemoveNewLine()}{Environment.NewLine}";
                }
            }
        }

        return sourceText;
    }

    public string GetFirstItem(string sourceText)
    {
        if (string.IsNullOrWhiteSpace(sourceText))
        {
            return sourceText;
        }

        var splitText = sourceText.SplitNewLine(StringSplitOptions.RemoveEmptyEntries);

        return splitText[0];
    }

    public List<VideoInfoDTO>? GetAllDataOrderByName()
    {
        var videoInfoDTOs = _videoInfoUseCases.GetAllVideoInfoContainsQueryHandler.Handle("");

        if (videoInfoDTOs is null || videoInfoDTOs.Count() == 0)
        {
            return null;
        }

        return videoInfoDTOs.OrderBy(c => c.Name)
                            .ToList();
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


    private string GetFileNameOnly(string fileName)
    {
        fileName = RemovePath(fileName);
        fileName = RemoveExtension(fileName);

        return fileName;
    }

    private string RemovePath(string fileName)
    {
        int pos = fileName.LastIndexOf("\\");
        fileName = fileName.Substring(pos + 1, fileName.Length - pos - 1);

        return fileName;
    }

    private string RemoveExtension(string fileName)
    {
        int pos = fileName.LastIndexOf(".");
        if (pos > 0)
        {
            fileName = fileName.Substring(0, pos);
        }

        return fileName;
    }

    private IEnumerable<VideoInfoDTO> GetVideoInfoWithConfigurationNames(IEnumerable<VideoInfoDTO> videoInfoDTOs)
    {
        var videoInfoWithConfigurationNames = new List<VideoInfoDTO>();

        foreach (var videoInfo in videoInfoDTOs)
        {
            var videoInfoWithConfigurationName = new VideoInfoDTO
            {
                Id = videoInfo.Id,
                Name = videoInfo.Name,
                Status = GetVideoInfoStatusByStatusName(videoInfo.Status).ConfigurationName
            };

            videoInfoWithConfigurationNames.Add(videoInfoWithConfigurationName);
        }

        return videoInfoWithConfigurationNames;
    }

    private VideoInfoStatus GetVideoInfoStatusByStatusName(string statusName)
    {
        VideoInfoStatus? videoinfoStatus = _videoInfoStatuses.FirstOrDefault(c => c.StatusName.Equals(statusName));
        if (videoinfoStatus is null)
            return new VideoInfoStatus();

        return videoinfoStatus;
    }

    private void InitializeVideoInfoStatuses()
    {
        string[]? configurationStatusNames = _configuration.GetSection("StatusNames").Get<string[]>();
        var statuses = Enum.GetValues(typeof(VideoInfoStatusEnum))
                           .Cast<VideoInfoStatusEnum>();

        foreach (var (status, index) in statuses.Select((status, index) => (status, index)))
        {
            var videoInfoStatus = new VideoInfoStatus
            {
                ConfigurationName = status.ToString(),
                StatusName = status.ToString(),
                Status = status
            };

            if (configurationStatusNames?.Length > index)
            {
                videoInfoStatus.ConfigurationName = configurationStatusNames[index];
            }

            _videoInfoStatuses.Add(videoInfoStatus);
        }
    }

    private string[] GetStatusNames(string[]? configurationStatusNames)
    {
        var statusNames = Enum.GetNames(typeof(VideoInfoStatusEnum)).ToArray();

        if (configurationStatusNames?.Length == statusNames.Length)
        {
            for (int i = 0; i < statusNames.Length; i++)
            {
                statusNames[i] = configurationStatusNames[i];
            }
        }

        return statusNames;
    }

}
