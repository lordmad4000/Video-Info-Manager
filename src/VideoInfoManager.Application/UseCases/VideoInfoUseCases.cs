using VideoInfoManager.Application.UseCases.VideoInfoCommands;
using VideoInfoManager.Application.UseCases.VideoInfoQueries;

namespace VideoInfoManager.Application.UseCases;

public record VideoInfoUseCases(
    CreateManyVideoInfoCommandHandler CreateManyVideoInfoCommandHandler,
    CreateVideoInfoCommandHandler CreateVideoInfoCommandHandler,
    DeleteVideoInfoCommandHandler DeleteVideoInfoCommandHandler,
    UpdateVideoInfoCommandHandler UpdateVideoInfoCommandHandler,
    GetAllVideoInfoContainsNameListQueryHandler GetAllVideoInfoContainsNameListQueryHandler,
    GetAllVideoInfoContainsQueryHandler GetAllVideoInfoContainsQueryHandler);

// TODO CREATE CUSTOM LOGGER
//private void SaveLog(string data)
//{
//    var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.log");

//    File.AppendAllText(fileName, $"{DateTime.UtcNow}{Environment.NewLine}{data}");
//}
