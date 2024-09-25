using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Domain.Enums;
using VideoInfoManager.Presentation.CrossCutting.Models;

namespace VideoInfoManager.Presentation.CrossCutting.Services
{
    public interface IVideoInfoManagerPresentationAppService
    {
        List<VideoInfoStatus> GetVideoInfoStatuses();
        List<VideoInfoDTO> GetResults();
        void LastSearch(List<VideoInfoStatusEnum> videoInfoActiveStatuses, bool isVideoName = false);
        void Search(string[]? search, List<VideoInfoStatusEnum> videoInfoActiveStatuses, bool isVideoName = false);
        string? ProcessData(string? textData, string status);
        bool Update(VideoInfoDTO videoInfoDTO);
        string NormalizeFileName(string fileName);
        VideoInfoStatus GetVideoInfoStatusByConfigurationName(string configurationName);
    }
}