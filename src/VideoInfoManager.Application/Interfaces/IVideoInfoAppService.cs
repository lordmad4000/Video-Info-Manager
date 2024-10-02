using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Domain.Enums;

namespace VideoInfoManager.Application.Interfaces;

public interface IVideoInfoAppService
{
    bool Add(VideoInfoDTO videoInfoDTO);
    bool Update(VideoInfoDTO videoInfoDTO);
    string AddManyFromStringArray(string[] fileNames, VideoInfoStatusEnum status);
    IEnumerable<VideoInfoDTO> GetManyContains(string search);
    IEnumerable<VideoInfoDTO> GetManyContainsNameList(List<string> nameList);
    bool Remove(Guid id);
}