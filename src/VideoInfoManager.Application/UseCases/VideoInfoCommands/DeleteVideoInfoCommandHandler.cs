using VideoInfoManager.Domain.Interfaces;
using VideoInfoManager.Domain.Models;

namespace VideoInfoManager.Application.UseCases.VideoInfoCommands;

public class DeleteVideoInfoCommandHandler
{
    private readonly IVideoInfoRepository _videoInfoRepository;

    public DeleteVideoInfoCommandHandler(IVideoInfoRepository videoInfoRepository)
    {
        _videoInfoRepository = videoInfoRepository;
    }

    public bool Handle(Guid id)
    {
        bool result = false;
        VideoInfo? videoInfo = _videoInfoRepository.Remove(id);
        if (videoInfo is not null)
        {
            result = _videoInfoRepository.SaveChanges() == 1;
            //TODO SaveLog($"-(Removed {videoInfo.Name}");
        }

        return result;
    }
}