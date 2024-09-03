using Microsoft.Extensions.DependencyInjection;
using VideoInfoManager.Presentation.Wpf.Configuration;

namespace VideoInfoManager.Presentation.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
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

    //protected override void OnStartup(StartupEventArgs e)
    //{
    //    base.OnStartup(e);
    //    Console.WriteLine("PEPE");
    //    // here you take control
    //}
}
