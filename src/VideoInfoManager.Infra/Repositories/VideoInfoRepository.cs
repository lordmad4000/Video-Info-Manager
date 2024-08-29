using Microsoft.EntityFrameworkCore;
using VideoInfoManager.Domain.Enums;
using VideoInfoManager.Domain.Interfaces;
using VideoInfoManager.Domain.Models;
using VideoInfoManager.Infra.Context;

namespace VideoInfoManager.Infra.Repositories;

public class VideoInfoRepository : IVideoInfoRepository
{
    private readonly VideoInfoContext _context;
    private readonly DbSet<VideoInfo> _dbSetvideoInfo;

    public VideoInfoRepository(VideoInfoContext context)
    {
        _context = context;
        _dbSetvideoInfo = _context.Set<VideoInfo>();
    }

    public void Add(VideoInfo videoInfo)
    {
        _dbSetvideoInfo.Add(videoInfo);
    }

    public void Update(VideoInfo videoInfo)
    {
        _dbSetvideoInfo.Update(videoInfo);
    }

    public VideoInfo? GetById(Guid id)
    {
        var videoInfo = _dbSetvideoInfo.FirstOrDefault(c => c.Id == id);

        return videoInfo;
    }

    public VideoInfo? GetByName(string name)
    {
        var videoInfo = _dbSetvideoInfo.FirstOrDefault(c => c.Name.ToLower() == name.ToLower());

        return videoInfo;
    }

    public IEnumerable<VideoInfo> GetManyContains(string name)
    {
        var videoInfo = new List<VideoInfo>();
        try
        {
            videoInfo = _dbSetvideoInfo.Where(c => c.Name.ToLower().Contains(name.ToLower()))
                                       .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return videoInfo;
    }

    public IEnumerable<VideoInfo> GetManyByStatus(VideoInfoStatusEnum status)
    {
        var videoInfo = new List<VideoInfo>();
        try
        {
            videoInfo = _dbSetvideoInfo.Where(c => c.Status == status)
                                       .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return videoInfo;
    }

    public IEnumerable<VideoInfo> GetAll()
    {
        var videosInfo = _dbSetvideoInfo.ToList();

        return videosInfo;
    }

    public VideoInfo? Remove(Guid id)
    {
        var videoInfo = GetById(id);
        if (videoInfo is null)
            return null;

        _context.Remove(videoInfo);

        return videoInfo;
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

}
