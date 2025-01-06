using System.Collections.Concurrent;
using System.Security.Cryptography;
using Telegram.Bot.Types;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Data.Repositories.Abstract;
using WetPicsRebirth.Exceptions;
using WetPicsRebirth.Extensions;
using WetPicsRebirth.Infrastructure;
using WetPicsRebirth.Infrastructure.ImageProcessing;
using WetPicsRebirth.Infrastructure.Models;
using WetPicsRebirth.Services;

namespace WetPicsRebirth.Commands.ServiceCommands.Posting;

public record PostNext : IRequest;

public class PostNextHandler : IRequestHandler<PostNext>
{
    private static readonly ConcurrentDictionary<long, int> ScenePostNumbers = new();

    private readonly IActressesRepository _actressesRepository;
    private readonly ILogger<PostNextHandler> _logger;
    private readonly IScenesRepository _scenesRepository;
    private readonly IPopularListLoader _popularListLoader;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IPostedMediaRepository _postedMediaRepository;
    private readonly ITelegramPreparer _telegramPreparer;
    private readonly IModerationService _moderationService;
    private readonly IPauseService _pauseService;

    private readonly string _channelLink;
    private readonly string _accessLink;

    public PostNextHandler(
        IScenesRepository scenesRepository,
        IActressesRepository actressesRepository,
        ILogger<PostNextHandler> logger,
        IPopularListLoader popularListLoader,
        ITelegramBotClient telegramBotClient,
        IPostedMediaRepository postedMediaRepository,
        IConfiguration configuration,
        ITelegramPreparer telegramPreparer,
        IModerationService moderationService,
        IPauseService pauseService)
    {
        _scenesRepository = scenesRepository;
        _actressesRepository = actressesRepository;
        _logger = logger;
        _popularListLoader = popularListLoader;
        _telegramBotClient = telegramBotClient;
        _postedMediaRepository = postedMediaRepository;
        _telegramPreparer = telegramPreparer;
        _moderationService = moderationService;
        _pauseService = pauseService;
        _channelLink = configuration.GetRequiredValue<string>("ChannelInviteLink");
        _accessLink = configuration.GetRequiredValue<string>("AccessLink");
    }

    public async Task Handle(PostNext request, CancellationToken cancellationToken)
    {
        var readyScenes = await _scenesRepository.GetEnabledAndReady();

        foreach (var scene in readyScenes)
        {
            if (_pauseService.IsPaused(scene))
                continue;

            var actresses = await _actressesRepository.GetForChat(scene.ChatId);

            if (actresses.Count == 0)
                continue;

            await PostNextForScene(scene, actresses);
        }
    }

    private async Task PostNextForScene(Scene scene, IReadOnlyCollection<Actress> actresses)
    {
        for (var i = 0; i < actresses.Count; i++)
        {
            var now = SystemClock.Instance.GetCurrentInstant();

            var postNumber = GetScenePostNumber(scene.ChatId);
            var selectedActress = actresses.ElementAt(postNumber % actresses.Count);

            try
            {
                await PostNextForActress(selectedActress);
                await _scenesRepository.SetPostedAt(scene.ChatId, now);
                break;
            }
            catch (NoNewPostsInActressException e)
            {
                _logger.LogInformation(
                    e,
                    "Unable to post for actress {ImageSource} in chat {ChatId}",
                    selectedActress.ImageSource,
                    selectedActress.ChatId);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "Unable to post for actress {ImageSource} in chat {ChatId}",
                    selectedActress.ImageSource,
                    selectedActress.ChatId);
            }
        }
    }

    private async Task PostNextForActress(Actress actress, params int[] skipIds)
    {
        var list = await _popularListLoader.Load(actress.ImageSource, actress.Options);
        var postIds = list.Where(x => !skipIds.Contains(x.Id)).Select(x => (x.Id, x.Md5Hash)).ToList();

        var newId = await _postedMediaRepository.GetFirstNew(actress.ChatId, actress.ImageSource, postIds);

        if (newId == null)
            throw new NoNewPostsInActressException();

        var loadedPost = await _popularListLoader.LoadPost(actress.ImageSource, list.First(x => x.Id == newId.Value));

        if (loadedPost.RequireModeration)
        {
            var approved = await _moderationService.CheckPost(loadedPost.Post);
            if (!approved)
            {
                var newSkipIds = skipIds.Append(loadedPost.Post.PostHeader.Id).ToArray();
                await PostNextForActress(actress, newSkipIds);
                return;
            }
        }

        if (ShouldSkip(loadedPost))
        {
            var newSkipIds = skipIds.Append(loadedPost.Post.PostHeader.Id).ToArray();
            await PostNextForActress(actress, newSkipIds);
            return;
        }

        var post = loadedPost.Post;
        var hash = post.PostHeader.Md5Hash ?? GetHash(post.File);

        var (sentPost, fileId, fileType) = await SentPostToTelegram(actress, post);

        await _postedMediaRepository.Add(
            actress.ChatId,
            sentPost.MessageId,
            fileId,
            fileType,
            actress.ImageSource,
            post.PostHeader.Id,
            hash);

        await post.File.DisposeAsync();
    }

    private static bool ShouldSkip(LoadedPost loadedPost)
        => loadedPost.Post is BannedPost
           || loadedPost.Post.FileSize > 50_000_000
           || loadedPost.Post.FileName.EndsWith(".webm");

    private async Task<(Message sentPost, string fileId, MediaType fileType)> SentPostToTelegram(
        Actress actress,
        Post post)
    {
        try
        {
            var caption = _popularListLoader.CreateCaption(actress.ImageSource, actress.Options, post);
            caption = EnrichCaption(caption);
            var ((sentPost, fileId), fileType) = post.Type switch
            {
                MediaType.Photo => (await SendPhoto(actress, post, caption), MediaType.Photo),
                MediaType.Video => (await SendVideo(actress, post, caption), MediaType.Video),
                _ => throw new ArgumentOutOfRangeException(nameof(post.Type))
            };
            return (sentPost, fileId, fileType);

        }
        catch (Exception e)
        {
            throw new UnableToPostForActressException(post.PostHeader.Id, e);
        }
    }

    private async Task<(Message sentPost, string fileId)> SendPhoto(Actress actress, Post post, string caption)
    {
        await using var file = _telegramPreparer.Prepare(post.File, post.FileSize);

        var sentPost = await _telegramBotClient.SendPhoto(
            chatId: actress.ChatId,
            photo: InputFile.FromStream(file, post.FileName),
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

    private async Task<(Message sentPost, string fileId)> SendVideo(Actress actress, Post post, string caption)
    {
        var sentPost = await _telegramBotClient.SendVideo(
            actress.ChatId,
            InputFile.FromStream(post.File, post.FileName),
            caption: caption,
            parseMode: ParseMode.Html,
            replyMarkup: Keyboards.WithLikes(0));

        var fileId = sentPost.Video!.FileId;

        return (sentPost, fileId);
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

    private static int GetScenePostNumber(long sceneId)
    {
        ScenePostNumbers.AddOrUpdate(sceneId, 0, (_, old) => old + 1);
        return ScenePostNumbers[sceneId];
    }
}
