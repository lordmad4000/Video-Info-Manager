using VideoInfoManager.Domain.Enums;
using VideoInfoManager.Domain.Models;

namespace VideoInfoManager.Application.DTOs;

public class VideoInfoDTO
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public static VideoInfoDTO Map(VideoInfo videoInfo)
    {
        return new VideoInfoDTO
        {
            Id = videoInfo.Id,
            Name = videoInfo.Name,
            Status = videoInfo.Status.ToString(),
        };
    }

    public static VideoInfo Map(VideoInfoDTO videoInfoDTO)
    {
        VideoInfoStatusEnum status = videoInfoDTO.StatusToVideoInfoStatusEnum();

        return new VideoInfo(videoInfoDTO.Name, status);
    }

    public VideoInfoStatusEnum StatusToVideoInfoStatusEnum()
    {
        VideoInfoStatusEnum status = VideoInfoStatusEnum.Pended;
        Enum.TryParse<VideoInfoStatusEnum>(Status, out status);

        return status;
    }

    public static IEnumerable<VideoInfoDTO> Map(IEnumerable<VideoInfo> videosInfo)
    {
        var videosInfoDto = new List<VideoInfoDTO>();
        foreach (var videoInfo in videosInfo)
        {
            videosInfoDto.Add(Map(videoInfo));
        }

        return videosInfoDto;
    }

}
