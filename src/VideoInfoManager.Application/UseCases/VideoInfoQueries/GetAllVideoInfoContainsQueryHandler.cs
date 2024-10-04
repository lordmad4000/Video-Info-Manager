using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Domain.Interfaces;

namespace VideoInfoManager.Application.UseCases.VideoInfoQueries;

public class GetAllVideoInfoContainsQueryHandler
{
    private readonly IVideoInfoRepository _videoInfoRepository;

    public GetAllVideoInfoContainsQueryHandler(IVideoInfoRepository videoInfoRepository)
    {
        _videoInfoRepository = videoInfoRepository;
    }

    public IEnumerable<VideoInfoDTO> Handle(string search)
    {
        var videosInfo = _videoInfoRepository.GetManyContains(search);
        if (videosInfo is null)
        {
            return new List<VideoInfoDTO>();
        }

        return VideoInfoDTO.Map(videosInfo);
    }
}