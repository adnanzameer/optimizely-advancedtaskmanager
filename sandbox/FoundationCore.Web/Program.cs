using FoundationCore.Web.Helpers;
using Serilog;

namespace FoundationCore.Web;

public class Program
{
    public static void Main(string[] args)
    {
        if (EnvironmentHelper.IsLocal())
        {
            //Development configuration
        }

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Warning()
            .WriteTo.File("app_data/log.txt", rollingInterval: RollingInterval.Day)
            //.WriteTo.Udp("localhost", 878, AddressFamily.InterNetwork)
            .CreateLogger();

        CreateHostBuilder(args, EnvironmentHelper.IsLocal()).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args, bool isDevelopment)
    {
        if (isDevelopment)
        {
            //Development configuration can be addded here, like local logging.
            return Host.CreateDefaultBuilder(args)
                .ConfigureCmsDefaults()
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }

        return Host.CreateDefaultBuilder(args)
            .ConfigureCmsDefaults()
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
