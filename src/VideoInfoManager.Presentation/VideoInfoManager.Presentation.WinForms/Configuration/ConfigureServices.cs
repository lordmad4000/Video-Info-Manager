using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VideoInfoManager.Application.Interfaces;
using VideoInfoManager.Application.Services;
using VideoInfoManager.Domain.Interfaces;
using VideoInfoManager.Infra.Context;
using VideoInfoManager.Infra.Repositories;
using VideoInfoManager.Presentation.WinForms.Forms;

namespace VideoInfoManager.Presentation.WinForms.Configuration;

public static class ConfigureServices
{
    private static IConfiguration? Configuration;

    public static IServiceCollection AddConfiguration(IServiceCollection services)
    {
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        Configuration = builder.Build();

        services.AddSingleton<IConfiguration>(Configuration);

        return services;
    }

    public static IServiceCollection AddDbContext(IServiceCollection services)
    {
        var DataSource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VideoInfoDB");

        services.AddDbContext<VideoInfoContext>(options =>
        {
            options.UseSqlite($"Data Source={DataSource}");
        });

        return services;
    }

    public static void DataBaseEnsureCreated(ServiceProvider serviceProvider)
    {
        var client = serviceProvider.GetRequiredService<VideoInfoContext>();
        client.Database.EnsureCreated();
    }

    public static IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddScoped<FormSearch>();
        services.AddScoped<IVideoInfoAppService, VideoInfoAppService>();
        services.AddScoped<IVideoInfoManagerAppService, VideoInfoManagerAppService>();
        services.AddScoped<IVideoInfoRepository, VideoInfoRepository>();

        return services;
    }
}
