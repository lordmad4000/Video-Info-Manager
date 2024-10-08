﻿using System.Text;
using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Application.Interfaces;
using VideoInfoManager.Domain.Enums;
using VideoInfoManager.Domain.Interfaces;
using VideoInfoManager.Domain.Models;

namespace VideoInfoManager.Application.Services;

public class VideoInfoAppService : IVideoInfoAppService
{
    private readonly IVideoInfoRepository _videoInfoRepository;

    public VideoInfoAppService(IVideoInfoRepository videoInfoRepository)
    {
        _videoInfoRepository = videoInfoRepository;
    }

    public bool Add(VideoInfoDTO videoInfoDTO)
    {
        var videoInfo = VideoInfoDTO.Map(videoInfoDTO);
        _videoInfoRepository.Add(videoInfo);
        _videoInfoRepository.SaveChanges();

        return true;
    }

    public bool Update(VideoInfoDTO videoInfoDTO)
    {
        var videoInfo = _videoInfoRepository.GetById(videoInfoDTO.Id);
        if (videoInfo is null)
            return false;

        var logData = $"*(Updated to {videoInfoDTO.Status.ToString()}) {videoInfoDTO.Name} from ({videoInfo.Status.ToString()}) {videoInfo.Name}{Environment.NewLine}";

        videoInfo.Name = videoInfoDTO.Name;
        videoInfo.Status = videoInfoDTO.StatusToVideoInfoStatusEnum();
        _videoInfoRepository.Update(videoInfo);
        _videoInfoRepository.SaveChanges();

        SaveLog(logData);

        return true;
    }

    public string AddManyFromStringArray(string[] fileNames, VideoInfoStatusEnum status)
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

        SaveLog(results.ToString());
        return $"Added to [{status.ToString()}]: {added}{Environment.NewLine}Updated to [{status.ToString()}]: {updated}{Environment.NewLine}";
    }

    public IEnumerable<VideoInfoDTO> GetManyContains(string search)
    {
        var videosInfo = _videoInfoRepository.GetManyContains(search);
        if (videosInfo is null)
        {
            return new List<VideoInfoDTO>();
        }

        return VideoInfoDTO.Map(videosInfo);
    }

    public IEnumerable<VideoInfoDTO> GetManyContainsNameList(List<string> nameList)
    {
        var videosInfoDTO = new List<VideoInfoDTO>();
        foreach (var name in nameList)
        {
            if (String.IsNullOrEmpty(name) is false)
            {
                var videosInfo = _videoInfoRepository.GetManyContains(name.Trim());
                if (videosInfo != null)
                {
                    videosInfoDTO.AddRange(VideoInfoDTO.Map(videosInfo));
                }
            }
        }

        return videosInfoDTO;
    }

    public bool Remove(Guid id)
    {
        bool result = false;
        VideoInfo? videoInfo = _videoInfoRepository.Remove(id);
        if (videoInfo is not null)
        {
            result = _videoInfoRepository.SaveChanges() == 1;
            SaveLog($"-(Removed {videoInfo.Name}");
        }

        return result;
    }

    private void SaveLog(string data)
    {
        var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.log");

        File.AppendAllText(fileName, $"{DateTime.UtcNow}{Environment.NewLine}{data}");
    }

}
