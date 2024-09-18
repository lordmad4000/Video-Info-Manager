using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VideoInfoManager.Application.Interfaces;
using VideoInfoManager.Application.Services;
using VideoInfoManager.Domain.Interfaces;
using VideoInfoManager.Infra.Context;
using VideoInfoManager.Infra.Repositories;
using VideoInfoManager.Presentation.CrossCutting.Services;

namespace VideoInfoManager.Presentation.CrossCutting.Extensions;

public static class DependencyInjectionExtensions
{
    private static IServiceProvider? ServiceProvider { get; set; }

    public static object? GetService(Type serviceType)
    {
        return ServiceProvider is null
            ? null
            : ServiceProvider.GetService(serviceType);
    }

    public static T? GetService<T>()
    {
        return ServiceProvider is null
            ? default(T)
            : ServiceProvider.GetService<T>();
    }

    public static IServiceCollection AddDbContext(this IServiceCollection services)
    {
        var DataSource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VideoInfoDB");

        services.AddDbContext<VideoInfoContext>(options =>
        {
            options.UseSqlite($"Data Source={DataSource}");
        });

        return services;
    }

    public static ServiceProvider DataBaseEnsureCreated(this ServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        var client = serviceProvider.GetRequiredService<VideoInfoContext>();
        client.Database.EnsureCreated();

        return serviceProvider;
    }

    public static IServiceCollection AddPresentationCrosscuttingServices(this IServiceCollection services)
    {
        return services.AddSingleton<IVideoInfoManagerPresentationAppService, VideoInfoManagerPresentationAppService>();
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services.AddTransient<IVideoInfoAppService, VideoInfoAppService>()
                       .AddTransient<IVideoInfoManagerAppService, VideoInfoManagerAppService>();
    }

    public static IServiceCollection AddInfraServices(this IServiceCollection services)
    {
        return services.AddTransient<IVideoInfoRepository, VideoInfoRepository>();
    }


}
