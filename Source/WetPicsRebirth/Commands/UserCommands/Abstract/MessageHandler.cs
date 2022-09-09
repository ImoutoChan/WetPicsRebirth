using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using WetPicsRebirth.EntryPoint.Service.Notifications;

namespace WetPicsRebirth.Commands.UserCommands.Abstract;

public abstract class MessageHandler : IMessageHandler
{
    private const string MeMemoryCacheKey = "_meMemoryCacheKey";

    private readonly IMemoryCache _memoryCache;
    private readonly ILogger _logger;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly string _handlerName;

    protected MessageHandler(
        ITelegramBotClient telegramBotClient,
        ILogger logger,
        IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _telegramBotClient = telegramBotClient;
        _logger = logger;

        _handlerName = GetType().FullName!;
    }

    public async Task Handle(MessageNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            var command = await GetCommand(notification.Message, cancellationToken);

            if (!WantHandle(notification.Message, command))
                return;

            _logger.LogInformation("Command received {Command} by {Handler}", command, _handlerName);
            await Handle(notification.Message, command, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "An error occurred while processing message {Message} {ChatId} {UserId}",
                notification.Message.Text,
                notification.Message.Chat.Id,
                notification.Message.From?.Id);
        }
    }

    private async Task<string?> GetCommand(Message message, CancellationToken cancellationToken)
    {
        var me = await GetUser(cancellationToken);
        var botUsername = me.Username!;

        var text = message.Text;

        if (string.IsNullOrWhiteSpace(text) || !text.StartsWith("/"))
            return null;

        var firstWord = text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).First();
        var isCommandWithId = firstWord.Contains('@') && firstWord.EndsWith(botUsername);
        var command = isCommandWithId ? firstWord.Split('@').First() : firstWord;

        return command;
    }

    private async Task<User> GetUser(CancellationToken cancellationToken)
    {
        return await _memoryCache.GetRequiredOrCreateAsync(
            MeMemoryCacheKey,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                return _telegramBotClient.GetMeAsync(cancellationToken);
            });
    }

    public virtual IEnumerable<string> ProvidedCommands => Array.Empty<string>();

    protected abstract bool WantHandle(Message message, string? command);

    protected abstract Task Handle(Message message, string? command, CancellationToken cancellationToken);

    protected async Task<bool> CheckOnAdmin(long targetChatId, long userId)
    {
        try
        {
            var admins = await _telegramBotClient.GetChatAdministratorsAsync(new ChatId(targetChatId));

            var isAdmin = admins.FirstOrDefault(x => x.User.Id == userId);

            return isAdmin is ChatMemberAdministrator
            {
                Status: ChatMemberStatus.Administrator,
                CanPostMessages: true
            } or ChatMemberOwner;
        }
        catch (ApiRequestException ex)
            when (ex.Message == "Bad Request: there is no administrators in the private chat")
        {
            // target chat is private chat
            return true;
        }
        catch
        {
            return false;
        }
    }
}
