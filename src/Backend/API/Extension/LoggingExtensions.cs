using Serilog;

namespace API.Extension
{
    public static class LoggingExtensions
    {
        public static void ConfigureSerilog(this IHostBuilder hostBuilder)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .Enrich.FromLogContext()
                .MinimumLevel.Information()
                .CreateLogger();

            hostBuilder.UseSerilog();
        }
    }
}
