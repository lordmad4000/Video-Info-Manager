﻿using VideoInfoManager.Domain.Enums;

namespace VideoInfoManager.Presentation.CrossCutting.Models;

public class VideoInfoStatus
{
    public string ConfigurationName { get; set; } = "Pended";
    public string StatusName { get; set; } = "Pended";
    public VideoInfoStatusEnum Status { get; set; } = VideoInfoStatusEnum.Pended;
}
