using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Domain.Interfaces;

namespace VideoInfoManager.Application.UseCases.VideoInfoQueries;

public class GetAllVideoInfoContainsNameListQueryHandler
{
    private readonly IVideoInfoRepository _videoInfoRepository;

    public GetAllVideoInfoContainsNameListQueryHandler(IVideoInfoRepository videoInfoRepository)
    {
        _videoInfoRepository = videoInfoRepository;
    }

    public IEnumerable<VideoInfoDTO> Handle(List<string> nameList)
    {
        var videosInfoDTO = new List<VideoInfoDTO>();
        foreach (var name in nameList)
        {
            if (String.IsNullOrEmpty(name) is false)
            {
                var videosInfo = _videoInfoRepository.GetManyContains(name.Trim());
                if (videosInfo != null)
                {
                    videosInfoDTO.AddRange(VideoInfoDTO.Map(videosInfo));
                }
            }
        }

        return videosInfoDTO;
    }
}