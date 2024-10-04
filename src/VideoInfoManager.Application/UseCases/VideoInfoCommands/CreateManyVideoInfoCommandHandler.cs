using System.Text;
using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Domain.Enums;
using VideoInfoManager.Domain.Interfaces;

namespace VideoInfoManager.Application.UseCases.VideoInfoCommands;

public class CreateManyVideoInfoCommandHandler
{
    private readonly IVideoInfoRepository _videoInfoRepository;

    public CreateManyVideoInfoCommandHandler(IVideoInfoRepository videoInfoRepository)
    {
        _videoInfoRepository = videoInfoRepository;
    }

    public string Handle(string[] fileNames, VideoInfoStatusEnum status)
    {
        var results = new StringBuilder();
        int added = 0;
        int updated = 0;

        foreach (var file in fileNames)
        {
            if (file.ToLower() != "zz")
            {
                var videoInfoDTO = new VideoInfoDTO
                {
                    Name = file,
                    Status = status.ToString()
                };
                var videoInfoInDB = _videoInfoRepository.GetByName(videoInfoDTO.Name);
                int maxLength = videoInfoDTO.Name.Length > 60
                                ? 60
                                : videoInfoDTO.Name.Length;

                if (videoInfoInDB != null)
                {
                    videoInfoInDB.Status = videoInfoDTO.StatusToVideoInfoStatusEnum();
                    _videoInfoRepository.Update(videoInfoInDB);
                    results.Append($"*(Updated to {status.ToString()}) ");
                    updated++;
                }
                else
                {
                    var videoInfo = VideoInfoDTO.Map(videoInfoDTO);
                    _videoInfoRepository.Add(videoInfo);
                    results.Append($"+(Added to {status.ToString()}) ");
                    added++;
                }

                results.Append(videoInfoDTO.Name, 0, maxLength);
                results.AppendLine();
            }
        }

        _videoInfoRepository.SaveChanges();

        //TODO SaveLog(results.ToString());
        return $"Added to [{status.ToString()}]: {added}{Environment.NewLine}Updated to [{status.ToString()}]: {updated}{Environment.NewLine}";
    }
}