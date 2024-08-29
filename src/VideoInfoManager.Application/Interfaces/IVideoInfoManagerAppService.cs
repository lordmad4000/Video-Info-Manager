using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Application.Models;
using VideoInfoManager.Domain.Enums;

namespace VideoInfoManager.Application.Interfaces;

public interface IVideoInfoManagerAppService
{
    string? ProcessData(string? textData, VideoInfoStatusEnum status);
    IEnumerable<VideoInfoDTO> GetManyVideoInfo(ICollection<string> videoInfoNames, VideoInfoRenameConfiguration[]? videoInfoRenameConfigurations);
    IEnumerable<string> GetOnlyAuthors(ICollection<string> videoInfoNames, VideoInfoRenameConfiguration[]? videoInfoRenameConfigurations);
    bool IsMultipleAuthor(string fullName, VideoInfoRenameConfiguration[]? videoInfoRenameConfigurations);
    string RenameVideoInfoName(string videoInfoName, VideoInfoRenameConfiguration[]? videoInfoRenameConfigurations);
    string SubstringOfString(string text, int startIndex, int length = 0);
}