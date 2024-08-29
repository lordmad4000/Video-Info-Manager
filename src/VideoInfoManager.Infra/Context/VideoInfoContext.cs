using Microsoft.EntityFrameworkCore;
using VideoInfoManager.Domain.Models;

namespace VideoInfoManager.Infra.Context;

public class VideoInfoContext : DbContext
{
    public DbSet<VideoInfo> VideosInfo { get; set; }

    public VideoInfoContext(DbContextOptions<VideoInfoContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

}
