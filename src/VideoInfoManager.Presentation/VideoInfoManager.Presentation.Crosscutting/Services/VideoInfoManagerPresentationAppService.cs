using Microsoft.Extensions.Configuration;
using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Application.Interfaces;
using VideoInfoManager.Application.Models;
using VideoInfoManager.Domain.Enums;
using VideoInfoManager.Presentation.CrossCutting.Models;

namespace VideoInfoManager.Presentation.CrossCutting.Services;


public class VideoInfoManagerPresentationAppService : IVideoInfoManagerPresentationAppService
{

    private const int MinSearchLength = 3;

    private List<VideoInfoStatus> _videoInfoStatuses = new List<VideoInfoStatus>();
    private string[]? _lastSearch { get; set; } = null;
    private IEnumerable<VideoInfoDTO>? _lastSearchData { get; set; } = null;
    private VideoInfoDTO? _videoInfoSelected { get; set; } = null;
    private List<VideoInfoDTO> _results = new List<VideoInfoDTO>();

    private readonly IVideoInfoAppService _videoInfoAppService;
    private readonly IVideoInfoManagerAppService _videoInfoManagerAppService;
    private readonly IConfiguration _configuration;
    private readonly VideoInfoRenameConfiguration[]? _videoInfoRenameConfigurations;


    public VideoInfoManagerPresentationAppService(IVideoInfoAppService videoInfoAppService, IVideoInfoManagerAppService videoInfoManagerAppService, IConfiguration configuration)
    {
        _videoInfoAppService = videoInfoAppService;
        _videoInfoManagerAppService = videoInfoManagerAppService;
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
                     ? _videoInfoManagerAppService.GetManyVideoInfo(search, _videoInfoRenameConfigurations)
                     : _videoInfoAppService.GetManyContainsNameList(search.ToList());

        if (_lastSearchData is not null)
        {
            var data = _lastSearchData.Where(c => videoInfoActiveStatuses.Contains(c.StatusToVideoInfoStatusEnum()))
                                      .ToList();

            var multipleAuthors = data.Where(c => _videoInfoManagerAppService.IsMultipleAuthor(c.Name, _videoInfoRenameConfigurations));
            var singleAuthor = data.Where(c => _videoInfoManagerAppService.IsMultipleAuthor(c.Name, _videoInfoRenameConfigurations) is false);
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
                results = _videoInfoAppService.AddManyFromStringArray(files, videoInfoStatus);
            }
        }
        catch (Exception ex)
        {
            results = ex.Message;
        }

        return results;
    }

    public bool Update(VideoInfoDTO videoInfoDTO)
    {
        videoInfoDTO.Status = GetVideoInfoStatusByConfigurationName(videoInfoDTO.Status).StatusName;

        return _videoInfoAppService.Update(videoInfoDTO);
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

    private VideoInfoStatus GetVideoInfoStatusByStatusName(string statusName)
    {
        VideoInfoStatus? videoinfoStatus = _videoInfoStatuses.FirstOrDefault(c => c.StatusName.Equals(statusName));
        if (videoinfoStatus is null)
            return new VideoInfoStatus();

        return videoinfoStatus;
    }

}
