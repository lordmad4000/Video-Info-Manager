using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using VideoInfoManager.Application.Interfaces;
using VideoInfoManager.Application.Services;
using VideoInfoManager.Domain.Interfaces;
using VideoInfoManager.Infra.Context;
using VideoInfoManager.Infra.Repositories;
using VideoInfoManager.Presentation.Wpf.Helpers;
using VideoInfoManager.Presentation.Wpf.Services;
using VideoInfoManager.Presentation.Wpf.ViewModels;
using VideoInfoManager.Presentation.Wpf.Views;
using VideoInfoManager.Presentation.Wpf.Windows;

namespace VideoInfoManager.Presentation.Wpf.Configuration;

public static class ConfigureServices
{
    private static IConfiguration? Configuration;
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

    public static IServiceCollection AddConfiguration(this IServiceCollection services)
    {
        var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        Configuration = builder.Build();

        services.AddSingleton<IConfiguration>(Configuration);

        return services;
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

    public static void DataBaseEnsureCreated(this ServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        var client = serviceProvider.GetRequiredService<VideoInfoContext>();
        client.Database.EnsureCreated();

        VideoInfoSearchViewModel? _videoInfoSearchViewModel = serviceProvider.GetService<VideoInfoSearchViewModel>();
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddSingleton<MainWindow>()
                       .AddWindowFactory<EditDialogWindow>()
                       .AddSingleton<SearchService>()
                       .AddTransient<IVideoInfoAppService, VideoInfoAppService>()
                       .AddTransient<IVideoInfoManagerAppService, VideoInfoManagerAppService>()
                       .AddTransient<IVideoInfoRepository, VideoInfoRepository>()
                       .AddSingleton<MainWindowViewModel>()
                       .AddSingleton<VideoInfoSearchView>()
                       .AddSingleton<VideoInfoSearchViewModel>();
   }

    public static IServiceCollection AddWindowFactory<TWindow> (this IServiceCollection services)
        where TWindow : class
    {
        services.AddTransient<TWindow>();
        services.AddSingleton<Func<TWindow>>(c => () => c.GetService<TWindow>()!);
        services.AddSingleton<IAbstractFactory<TWindow>, AbstractFactory<TWindow>>();

        return services;
    }
}
