using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Domain.Interfaces;

namespace VideoInfoManager.Application.UseCases.VideoInfoCommands;

public class UpdateVideoInfoCommandHandler
{
    private readonly IVideoInfoRepository _videoInfoRepository;

    public UpdateVideoInfoCommandHandler(IVideoInfoRepository videoInfoRepository)
    {
        _videoInfoRepository = videoInfoRepository;
    }

    public bool Handle(VideoInfoDTO videoInfoDTO)
    {
        var videoInfo = _videoInfoRepository.GetById(videoInfoDTO.Id);
        if (videoInfo is null)
            return false;

        //TODO var logData = $"*(Updated to {videoInfoDTO.Status.ToString()}) {videoInfoDTO.Name} from ({videoInfo.Status.ToString()}) {videoInfo.Name}{Environment.NewLine}";

        videoInfo.Name = videoInfoDTO.Name;
        videoInfo.Status = videoInfoDTO.StatusToVideoInfoStatusEnum();
        _videoInfoRepository.Update(videoInfo);
        _videoInfoRepository.SaveChanges();

        //TODO SaveLog(logData);

        return true;
    }
}