using Microsoft.Extensions.DependencyInjection;
using VideoInfoManager.Presentation.Wpf.Configuration;

namespace VideoInfoManager.Presentation.Wpf;

internal static class Program
{

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main()
    {
        var app = new VideoInfoManager.Presentation.Wpf.App();

        var services = new ServiceCollection();
        ConfigureServices.AddDbContext(services);
        ConfigureServices.AddConfiguration(services);
        ConfigureServices.AddServices(services);

        using (ServiceProvider serviceProvider = services.BuildServiceProvider())
        {
            ConfigureServices.DataBaseEnsureCreated(serviceProvider);
            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            app.Run(mainWindow);
        }
    }
}
