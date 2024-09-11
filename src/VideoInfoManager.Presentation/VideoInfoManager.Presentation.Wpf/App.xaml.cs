using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using VideoInfoManager.Presentation.Wpf.Configuration;
using VideoInfoManager.Presentation.Wpf.Windows;

namespace VideoInfoManager.Presentation.Wpf;

public partial class App : System.Windows.Application
{
    public static IHost? AppHost { get; private set; }

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext()
                        .AddConfiguration()
                        .AddServices()
                        .AddWindowFactory<EditDialogWindow>()
                        .BuildServiceProvider().DataBaseEnsureCreated();
            })
            .Build();

        //var services = new ServiceCollection();
        //ConfigureServices.AddDbContext(services);
        //ConfigureServices.AddConfiguration(services);
        //ConfigureServices.AddServices(services);

        //using (ServiceProvider serviceProvider = services.BuildServiceProvider())
        //{
        //    ConfigureServices.DataBaseEnsureCreated(serviceProvider);
        //    var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        //    app.Run(mainWindow);
        //}
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
