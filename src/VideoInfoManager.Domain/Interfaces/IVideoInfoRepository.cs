using VideoInfoManager.Domain.Enums;
using VideoInfoManager.Domain.Models;

namespace VideoInfoManager.Domain.Interfaces;

public interface IVideoInfoRepository
{
    void Add(VideoInfo videoInfo);
    void Update(VideoInfo videoInfo);
    VideoInfo? GetById(Guid id);
    VideoInfo? GetByName(string name);
    IEnumerable<VideoInfo> GetManyContains(string name);
    IEnumerable<VideoInfo> GetManyByStatus(VideoInfoStatusEnum status);
    IEnumerable<VideoInfo> GetAll();
    VideoInfo? Remove(Guid id);
    int SaveChanges();
}