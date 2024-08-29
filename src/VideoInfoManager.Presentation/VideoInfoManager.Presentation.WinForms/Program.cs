using Microsoft.Extensions.DependencyInjection;
using VideoInfoManager.Presentation.WinForms.Configuration;
using VideoInfoManager.Presentation.WinForms.Forms;

namespace VideoInfoManager.Presentation.WinForms;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.SystemAware);
        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

        var services = new ServiceCollection();
        ConfigureServices.AddDbContext(services);
        ConfigureServices.AddConfiguration(services);
        ConfigureServices.AddServices(services);

        using (ServiceProvider serviceProvider = services.BuildServiceProvider())
        {
            ConfigureServices.DataBaseEnsureCreated(serviceProvider);
            var formSearch = serviceProvider.GetRequiredService<FormSearch>();
            System.Windows.Forms.Application.Run(formSearch);
        }


    }
}