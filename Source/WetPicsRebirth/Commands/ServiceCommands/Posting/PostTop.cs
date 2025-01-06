using Telegram.Bot.Types;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Data.Repositories.Abstract;
using WetPicsRebirth.Services;

namespace WetPicsRebirth.Commands.ServiceCommands.Posting;

public record PostTop(TopType Type) : IRequest;

public enum TopType { Weekly, Monthly, Yearly }

public class PostTopHandler : IRequestHandler<PostTop>
{
    private readonly IScenesRepository _scenesRepository;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IVotesRepository _votesRepository;
    private readonly IPauseService _pauseService;

    public PostTopHandler(
        IScenesRepository scenesRepository,
        ITelegramBotClient telegramBotClient,
        IVotesRepository votesRepository, IPauseService pauseService)
    {
        _scenesRepository = scenesRepository;
        _telegramBotClient = telegramBotClient;
        _votesRepository = votesRepository;
        _pauseService = pauseService;
    }

    public async Task Handle(PostTop command, CancellationToken _)
    {
        var topType = command.Type;
        var forLastDays = topType switch
        {
            TopType.Weekly => 7,
            TopType.Monthly => 30,
            TopType.Yearly => 365,
            _ => 7
        };
        
        var scenes = await _scenesRepository.GetAll();
        
        foreach (var scene in scenes.Where(x => x.Enabled))
        {
            var posts = await _votesRepository.GetTop(10, forLastDays);
        
            if (posts.Count > 0)
                await PostTopForScene(topType, scene, posts);
        }
    }

    private async Task PostTopForScene(TopType topType, Scene scene, IReadOnlyCollection<PostedMedia> postedMedia)
    {
        var topMedia = postedMedia.Where(x => x.FileType != MediaType.Unknown).ToList();
        var caption = GetCaption(topType, topMedia);

        var fileIds = topMedia
            .Select((x, i) =>
            {
                var media = CreateMedia(x.FileType, x.FileId);

                if (i != 0) 
                    return media;
                
                media.Caption = caption;
                media.ParseMode = ParseMode.Html;
                return media;
            })
            .OfType<IAlbumInputMedia>()
            .ToList();
        
        if (!fileIds.Any())
            return;

        var topMessage = await _telegramBotClient.SendMediaGroup(scene.ChatId, fileIds);
        await _telegramBotClient.PinChatMessage(scene.ChatId, topMessage[0].MessageId);
        _pauseService.Pause(scene, topType);
    }

    private static string GetCaption(TopType topType, IReadOnlyCollection<PostedMedia> topMedia)
    {
        var typeCaption = topType switch
        {
            TopType.Monthly => "месяц",
            TopType.Weekly => "неделю",
            TopType.Yearly => "год",
            _ => throw new ArgumentOutOfRangeException(nameof(topType), topType, null)
        };
        
        var mediaLinks = topMedia.Select((x, i) =>
            $"<a href=\"https://t.me/c/{x.ChatId.ToString().Replace("-100", "")}/{x.MessageId}\">{i + 1}.</a>" +
            $" <a href=\"{GetLinkToPost(x.ImageSource, x.PostId)}\">{x.ImageSource.ToString().ToLower()}</a>");

        var caption = $"💙 #Популярное за {typeCaption} ~ " + string.Join(" | ", mediaLinks);
        return caption;
    }

    private static InputMedia CreateMedia(MediaType fileType, string fileId)
        => fileType == MediaType.Photo
            ? new InputMediaPhoto(new InputFileId(fileId))
            : new InputMediaVideo(new InputFileId(fileId));

    private static string GetLinkToPost(ImageSource source, int postId)
    {
        return source switch
        {
            ImageSource.Pixiv => $"https://www.pixiv.net/en/artworks/{postId}",
            ImageSource.Danbooru => $"https://danbooru.donmai.us/posts/{postId}",
            ImageSource.Yandere => $"https://yande.re/post/show/{postId}",
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }
}
