using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Domain.Interfaces;

namespace VideoInfoManager.Application.UseCases.VideoInfoCommands;

public class CreateVideoInfoCommandHandler
{
    private readonly IVideoInfoRepository _videoInfoRepository;

    public CreateVideoInfoCommandHandler(IVideoInfoRepository videoInfoRepository)
    {
        _videoInfoRepository = videoInfoRepository;
    }

    public bool Handle(VideoInfoDTO videoInfoDTO)
    {
        var videoInfo = VideoInfoDTO.Map(videoInfoDTO);
        _videoInfoRepository.Add(videoInfo);
        _videoInfoRepository.SaveChanges();

        return true;
    }
}