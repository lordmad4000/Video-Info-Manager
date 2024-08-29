using VideoInfoManager.Domain.Enums;

namespace VideoInfoManager.Domain.Models;

public class VideoInfo
{
    public VideoInfo(string name, VideoInfoStatusEnum status)
    {
        Id = Guid.NewGuid();
        Name = name;
        Status = status;
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public VideoInfoStatusEnum Status { get; set; } = VideoInfoStatusEnum.Pended;
}
