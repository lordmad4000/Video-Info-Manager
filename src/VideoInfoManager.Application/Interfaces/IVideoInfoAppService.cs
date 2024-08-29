using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Domain.Enums;

namespace VideoInfoManager.Application.Interfaces;

public interface IVideoInfoAppService
{
    bool Add(VideoInfoDTO videoInfoDTO);
    bool Update(VideoInfoDTO videoInfoDTO);
    bool AddFromString(string file, VideoInfoStatusEnum status);
    string AddManyFromStringArray(string[] fileNames, VideoInfoStatusEnum status);
    IEnumerable<VideoInfoDTO> GetManyContains(string search);
    IEnumerable<VideoInfoDTO> GetManyContains(string search, List<VideoInfoStatusEnum> statusToShow);
    IEnumerable<VideoInfoDTO> GetManyByStatus(VideoInfoStatusEnum status);
    IEnumerable<VideoInfoDTO> GetManyContainsNameList(List<string> nameList);
    bool Remove(Guid id);
    string NormalizeFileName(string fileName);
}