using VideoInfoManager.Domain.Interfaces;
using VideoInfoManager.Domain.Models;

namespace VideoInfoManager.Infra.EventHandlers;

public class CreateVideoInfoEventHandler : IEventHandler<VideoInfo>
{
    public void Raise(VideoInfo videoInfo)
    {
        var logData = $"*(Added to {videoInfo.Status.ToString()}) {videoInfo.Name}{Environment.NewLine}";
        SaveLog(logData);
    }

    private void SaveLog(string data)
    {
        var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.log");

        File.AppendAllText(fileName, $"{DateTime.UtcNow}{Environment.NewLine}{data}");
    }

}
