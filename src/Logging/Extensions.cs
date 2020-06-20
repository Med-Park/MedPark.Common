using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace MedPark.Common.Logging
{
    public static class Extensions
    {
        public static IHostBuilder UseLogging(this IHostBuilder webHostBuilder)
        {
            return webHostBuilder.UseSerilog((context, loggerConfiguration) =>
            {
                var appOptions = context.Configuration.GetOptions<AppOptions>("App");
                var seqOptions = context.Configuration.GetOptions<SeqOptions>("SeqOptions");
                var serilogOptions = context.Configuration.GetOptions<SerilogOptions>("Serilog");

                if (!Enum.TryParse<LogEventLevel>(serilogOptions.Level, true, out var level))
                {
                    level = LogEventLevel.Information;
                }

                loggerConfiguration.Enrich.FromLogContext()
                    .MinimumLevel.Is(level)
                    .Enrich.WithProperty("ApplicationName", appOptions.Name)
                    .WriteTo.Console()
                    .ReadFrom.Configuration(context.Configuration);

                if (seqOptions.Enabled)
                    loggerConfiguration.WriteTo.Seq(seqOptions.SeqServerUrl);
            });
        }

        private static void AddSeq(LoggerConfiguration loggerConfiguration, LogEventLevel level, SeqOptions seqOptions)
        {
            loggerConfiguration.WriteTo.Seq(seqOptions.SeqServerUrl);
        }
    }
}