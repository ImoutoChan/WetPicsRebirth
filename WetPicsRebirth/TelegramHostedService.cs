using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot;

namespace WetPicsRebirth
{
    public class TelegramHostedService : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Startup> _logger;
        private readonly ITelegramBotClient _telegramBotClient;

        public TelegramHostedService(
            ILogger<Startup> logger,
            IConfiguration configuration,
            ITelegramBotClient telegramBotClient)
        {
            _logger = logger;
            _configuration = configuration;
            _telegramBotClient = telegramBotClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var address = _configuration.GetValue<string>("WebHookAddress");

            _logger.LogInformation("Removing WebHook");
            await _telegramBotClient.DeleteWebhookAsync(cancellationToken);

            _logger.LogInformation("Setting WebHook to {Address}", address);
            await _telegramBotClient.SetWebhookAsync(
                address,
                maxConnections: 5,
                cancellationToken: cancellationToken);
            _logger.LogInformation("WebHook is set to {Address}", address);

            var webHookInfo = await _telegramBotClient.GetWebhookInfoAsync(cancellationToken);
            _logger.LogInformation("WebHook info: {Info}", JsonConvert.SerializeObject(webHookInfo));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _telegramBotClient.DeleteWebhookAsync(cancellationToken);
            _logger.LogInformation("WebHook removed");
        }
    }
}
