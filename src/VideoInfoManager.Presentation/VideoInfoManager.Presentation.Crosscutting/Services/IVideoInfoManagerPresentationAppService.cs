using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Application.Models;
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
        IEnumerable<VideoInfoDTO> GetManyVideoInfo(ICollection<string> videoInfoNames);
        bool Update(VideoInfoDTO videoInfoDTO);
        string NormalizeFileName(string fileName);
        VideoInfoStatus GetVideoInfoStatusByConfigurationName(string configurationName);
        IEnumerable<string> GetOnlyAuthors(ICollection<string> videoInfoNames);
        bool IsMultipleAuthor(string fullName);
        string RenameVideoInfoName(string videoInfoName);
        string SubstringOfString(string text, int startIndex, int length = 0);
        string RemoveFirstItem(string sourceText);
        List<VideoInfoDTO>? GetAllDataOrderByName();

    }
}