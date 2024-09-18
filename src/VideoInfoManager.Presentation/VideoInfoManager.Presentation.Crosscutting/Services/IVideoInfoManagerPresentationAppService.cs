using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Presentation.CrossCutting.Models;

namespace VideoInfoManager.Presentation.CrossCutting.Services
{
    public interface IVideoInfoManagerPresentationAppService
    {
        List<VideoInfoStatus> GetVideoInfoStatuses();
        List<VideoInfoDTO> GetResults();
        void LastSearch(bool isVideoName = false);
        void Search(string[]? search, bool isVideoName = false);
        bool Update(VideoInfoDTO videoInfoDTO);
    }
}