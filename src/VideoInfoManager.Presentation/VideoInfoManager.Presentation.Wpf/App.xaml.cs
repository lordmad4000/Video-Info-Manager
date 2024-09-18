using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using VideoInfoManager.Presentation.Wpf.Configuration;
using VideoInfoManager.Presentation.Wpf.Windows;
using VideoInfoManager.Presentation.CrossCutting.Extensions;
using VideoInfoManager.Presentation.Wpf.ViewModels;

namespace VideoInfoManager.Presentation.Wpf;

public partial class App : System.Windows.Application
{
    public static IHost? AppHost { get; private set; }

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
                      .ConfigureServices((hostContext, services) =>
                      {
                          var serviceProvider = services.AddPresentationCrosscuttingServices()
                                                        .AddApplicationServices()
                                                        .AddInfraServices()
                                                        .AddDbContext()
                                                        .AddConfiguration()
                                                        .AddServices()
                                                        .BuildServiceProvider()
                                                        .DataBaseEnsureCreated();

                          VideoInfoSearchViewModel? _videoInfoSearchViewModel = serviceProvider.GetService<VideoInfoSearchViewModel>();
                      })
                      .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        var startupWindow = AppHost.Services.GetRequiredService<MainWindow>();
        startupWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();

        base.OnExit(e);
    }

}
