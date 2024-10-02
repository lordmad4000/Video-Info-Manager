using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VideoInfoManager.Application.Interfaces;
using VideoInfoManager.Application.Services;
using VideoInfoManager.Domain.Interfaces;
using VideoInfoManager.Infra.Repositories;
using VideoInfoManager.Presentation.WinForms.Forms;

namespace VideoInfoManager.Presentation.WinForms.Configuration;

public static class ConfigureServices
{
    private static IConfiguration? Configuration;

    public static IServiceCollection AddConfiguration(this IServiceCollection services)
    {
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        Configuration = builder.Build();

        services.AddSingleton<IConfiguration>(Configuration);

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<FormSearch>();
        services.AddScoped<IVideoInfoAppService, VideoInfoAppService>();
        services.AddScoped<IVideoInfoRepository, VideoInfoRepository>();

        return services;
    }
}
