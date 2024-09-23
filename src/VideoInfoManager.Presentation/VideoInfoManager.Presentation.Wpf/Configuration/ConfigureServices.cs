using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VideoInfoManager.Presentation.Wpf.Helpers;
using VideoInfoManager.Presentation.Wpf.ViewModels;
using VideoInfoManager.Presentation.Wpf.Views;
using VideoInfoManager.Presentation.Wpf.Windows;

namespace VideoInfoManager.Presentation.Wpf.Configuration;

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
        return services.AddSingleton<MainWindow>()
                       .AddWindowFactory<EditDialogWindow>()
                       .AddSingleton<MainWindowViewModel>()
                       .AddSingleton<StatusCheckBoxesViewModel>()
                       .AddSingleton<VideoInfoAddDataViewModel>()
                       .AddSingleton<VideoInfoSearchViewModel>()
                       .AddSingleton<VideoInfoAddDataView>()
                       .AddSingleton<VideoInfoSearchResultsView>()
                       .AddSingleton<VideoInfoSearchStatusView>()
                       .AddSingleton<VideoInfoSearchView>();
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
