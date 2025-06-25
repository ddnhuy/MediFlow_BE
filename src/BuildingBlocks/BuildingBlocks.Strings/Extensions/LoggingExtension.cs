using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace BuildingBlocks.Strings.Extensions
{
    public static class LoggingExtension
    {
        public static IServiceCollection AddSeqLogging(
            this IServiceCollection services,
            string seqUrl = "http://seq:5341",
            string serviceName = "UnknownService",
            LogEventLevel minimumLevel = LogEventLevel.Information)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(minimumLevel)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceName", serviceName)
                .WriteTo.Console()
                .WriteTo.Seq(seqUrl)
                .CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(Log.Logger, dispose: true);
            });

            return services;
        }
    }
}