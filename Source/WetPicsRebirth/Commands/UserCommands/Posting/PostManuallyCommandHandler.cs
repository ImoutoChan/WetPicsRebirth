using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot.Types;
using WetPicsRebirth.Commands.UserCommands.Abstract;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Data.Repositories.Abstract;
using WetPicsRebirth.Exceptions;
using WetPicsRebirth.Extensions;
using WetPicsRebirth.Infrastructure;
using WetPicsRebirth.Infrastructure.ImageProcessing;
using WetPicsRebirth.Infrastructure.Models;
using WetPicsRebirth.Services;

namespace WetPicsRebirth.Commands.UserCommands.Posting;

public class PostManuallyCommandHandler : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IEngineFactory _engineFactory;
    private readonly IPostedMediaRepository _postedMediaRepository;
    private readonly ITelegramPreparer _telegramPreparer;
    private readonly IPopularListLoader _popularListLoader;
    private readonly string _channelLink;
    private readonly string _accessLink;

    public PostManuallyCommandHandler(
        ITelegramBotClient telegramBotClient,
        ILogger<PostManuallyCommandHandler> logger,
        IMemoryCache memoryCache,
        IEngineFactory engineFactory,
        IPostedMediaRepository postedMediaRepository,
        ITelegramPreparer telegramPreparer,
        IConfiguration configuration,
        IPopularListLoader popularListLoader)
        : base(telegramBotClient, logger, memoryCache)
    {
        _telegramBotClient = telegramBotClient;
        _engineFactory = engineFactory;
        _postedMediaRepository = postedMediaRepository;
        _telegramPreparer = telegramPreparer;
        _popularListLoader = popularListLoader;
        _channelLink = configuration.GetRequiredValue<string>("ChannelInviteLink");
        _accessLink = configuration.GetRequiredValue<string>("AccessLink");
    }

    public override IEnumerable<string> ProvidedCommands
        => "/post rule34 1234".ToEnumerable();

    protected override bool WantHandle(Message message, string? command) => command == "/post";

    protected override async Task Handle(Message message, string? command, CancellationToken cancellationToken)
    {
        var parameters = message.Text?.Split(' ') ?? Array.Empty<string>();

        if (parameters.Length != 4)
        {
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Команда должна выглядеть как /post -1001411191119 rule34 1234, " +
                "где первый параметр это айди чата или канала, " +
                "второй источник поста, " +
                "третий id поста в иточнике",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        if (!long.TryParse(parameters[1], out var targetId))
        {
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Неверный айди чата или канала",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }
        
        if (!TryGetImageSource(parameters[2], out var source))
        {
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Неверный источник, доступны: pixiv, yandere, danbooru, ???",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        if (!int.TryParse(parameters[3], out var postId))
        {
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Неверный айди поста в источнике",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        if (!await CheckOnAdmin(targetId, message.From!.Id))
        {
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "У вас должны быть права администратора в выбранном чате или канале",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        try
        {
            var post = await _engineFactory.Get(source).LoadPost(new(postId, null));
            
            var hash = post.Post.PostHeader.Md5Hash ?? GetHash(post.Post.File);

            var (sentPost, fileId, fileType) = await SentPostToTelegram(source, targetId, post.Post);

            await _postedMediaRepository.Add(
                targetId,
                sentPost.MessageId,
                fileId,
                fileType,
                source,
                postId,
                hash);

            await post.Post.File.DisposeAsync();
        }
        catch (Exception e)
        {
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Не удалось отправить пост: " 
                + e.Message 
                + Environment.NewLine
                + e.StackTrace 
                + Environment.NewLine
                + e.InnerException?.Message 
                + Environment.NewLine
                + e.InnerException?.StackTrace,
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
        }
    }

    private static bool TryGetImageSource(string sourceString, out ImageSource source)
    {
        source = default;

        if (!Enum.TryParse(typeof(ImageSource), sourceString, true, out var result))
            return false;

        source = (ImageSource)result!;
        return true;
    }

    private static string GetHash(Stream file)
    {
        file.Seek(0, SeekOrigin.Begin);

        using var md5 = MD5.Create();
        var result = string.Join("", md5.ComputeHash(file).Select(x => x.ToString("X2"))).ToLowerInvariant();
        file.Seek(0, SeekOrigin.Begin);

        return result;
    }

    private string EnrichCaption(string caption)
        => $"{caption} | <a href=\"{_channelLink}\">join</a> | <a href=\"{_accessLink}\">access</a>";

    private async Task<(Message sentPost, string fileId, MediaType fileType)> SentPostToTelegram(
        ImageSource source,
        long chatId,
        Post post)
    {
        try
        {
            var caption = _popularListLoader.CreateCaption(source, string.Empty, post);
            caption = EnrichCaption(caption);
            var ((sentPost, fileId), fileType) = post.Type switch
            {
                MediaType.Photo => (await SendPhoto(chatId, post, caption), MediaType.Photo),
                MediaType.Video => (await SendVideo(chatId, post, caption), MediaType.Video),
                _ => throw new ArgumentOutOfRangeException(nameof(post.Type))
            };
            return (sentPost, fileId, fileType);

        }
        catch (Exception e)
        {
            throw new UnableToPostForActressException(post.PostHeader.Id, e);
        }
    }

    private async Task<(Message sentPost, string fileId)> SendPhoto(long chatId, Post post, string caption)
    {
        await using var file = _telegramPreparer.Prepare(post.File, post.FileSize);

        var sentPost = await _telegramBotClient.SendPhotoAsync(
            chatId: chatId,
            photo: new InputFileStream(file),
            caption: caption,
            parseMode: ParseMode.Html,
            replyMarkup: Keyboards.WithLikes(0));

        var fileId = sentPost.Photo!
            .OrderByDescending(x => x.Height)
            .ThenByDescending(x => x.Width)
            .Select(x => x.FileId)
            .First();

        return (sentPost, fileId);
    }

    private async Task<(Message sentPost, string fileId)> SendVideo(long chatId, Post post, string caption)
    {
        var sentPost = await _telegramBotClient.SendVideoAsync(
            chatId,
            new InputFileStream(post.File),
            caption: caption,
            parseMode: ParseMode.Html,
            replyMarkup: Keyboards.WithLikes(0));

        var fileId = sentPost.Video!.FileId;

        return (sentPost, fileId);
    }
}
