using Serilog;
using Serilog.Events;

namespace SupportHub.Api.Extensions;

public static class LoggingExtensions
{
    public static ConfigureHostBuilder UseSupportHubSerilog(this ConfigureHostBuilder host)
    {
        host.UseSerilog((hostingContext, loggerConfiguration) =>
        {
            var env = hostingContext.HostingEnvironment;

            loggerConfiguration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Environment", env.EnvironmentName)
                .Enrich.WithProperty("Application", "SupportHub.Api")
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [CorrelationId:{CorrelationId}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [CorrelationId:{CorrelationId}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 30);

            if (env.IsDevelopment())
            {
                loggerConfiguration.MinimumLevel.Debug();
            }
        });

        return host;
    }
}
