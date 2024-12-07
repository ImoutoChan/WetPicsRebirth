using Newtonsoft.Json;
using WetPicsRebirth.Extensions;

namespace WetPicsRebirth.Services;

public class TelegramHostedService : IHostedService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TelegramHostedService> _logger;
    private readonly ITelegramBotClient _telegramBotClient;

    public TelegramHostedService(
        ILogger<TelegramHostedService> logger,
        IConfiguration configuration,
        ITelegramBotClient telegramBotClient)
    {
        _logger = logger;
        _configuration = configuration;
        _telegramBotClient = telegramBotClient;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var address = _configuration.GetRequiredValue<string>("WebHookAddress");

        _logger.LogInformation("Removing WebHook");
        await _telegramBotClient.DeleteWebhook(cancellationToken: cancellationToken);

        _logger.LogInformation("Setting WebHook to {Address}", address);
        await _telegramBotClient.SetWebhook(
            address,
            maxConnections: 5,
            cancellationToken: cancellationToken);
        _logger.LogInformation("WebHook is set to {Address}", address);

        var webHookInfo = await _telegramBotClient.GetWebhookInfo(cancellationToken);
        _logger.LogInformation("WebHook info: {Info}", JsonConvert.SerializeObject(webHookInfo));
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _telegramBotClient.DeleteWebhook(cancellationToken: cancellationToken);
        _logger.LogInformation("WebHook removed");
    }
}
