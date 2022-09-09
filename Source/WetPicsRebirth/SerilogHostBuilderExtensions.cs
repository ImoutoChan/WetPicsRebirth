using Serilog;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Exceptions;

namespace WetPicsRebirth;

public static class SerilogHostBuilderExtensions
{
    public static IHostBuilder ConfigureSerilog(
        this IHostBuilder hostBuilder,
        Action<LoggerConfiguration, IConfiguration>? configureLogger = null)
    {
        hostBuilder.ConfigureLogging(
            (context, builder) =>
            {
                builder.ClearProviders();
                builder.AddSerilog(
                    dispose: true,
                    logger: GetSerilogLogger(
                        context.Configuration,
                        (logger, configuration) =>
                        {
                            var botToken = configuration.GetRequiredValue<string>("TelegramApiKey");
                            var botModerator = configuration.GetRequiredValue<int>("ModeratorId");

                            logger
                                .Enrich.FromLogContext()
                                .MinimumLevel.Information()
                                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)

                                .WriteTo.Console()

                                .WriteTo.Telegram(
                                    botToken, 
                                    botModerator.ToString(),
                                    restrictedToMinimumLevel: LogEventLevel.Warning);

                            SelfLog.Enable(Console.Error);
                            configureLogger?.Invoke(logger, configuration);
                        }));
            });

        return hostBuilder;
    }

    private static Logger GetSerilogLogger(
        IConfiguration configuration,
        Action<LoggerConfiguration, IConfiguration> configureLogger)
    {
        var loggerBuilder = new LoggerConfiguration();

        configureLogger.Invoke(loggerBuilder, configuration);

        loggerBuilder.Enrich.WithProperty("InstanceId", Guid.NewGuid());
        loggerBuilder.Enrich.WithExceptionDetails();

        return loggerBuilder.CreateLogger();
    }
}
