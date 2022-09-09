using Telegram.Bot.Types;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Data.Repositories.Abstract;

namespace WetPicsRebirth.Commands.ServiceCommands.Posting;

public record PostWeeklyTop : IRequest;

public class PostWeeklyTopHandler : IRequestHandler<PostWeeklyTop>
{
    private readonly IScenesRepository _scenesRepository;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IVotesRepository _votesRepository;

    public PostWeeklyTopHandler(
        IScenesRepository scenesRepository,
        ITelegramBotClient telegramBotClient,
        IVotesRepository votesRepository)
    {
        _scenesRepository = scenesRepository;
        _telegramBotClient = telegramBotClient;
        _votesRepository = votesRepository;
    }

    public async Task<Unit> Handle(PostWeeklyTop _, CancellationToken __)
    {
        var scenes = await _scenesRepository.GetAll();
        
        foreach (var scene in scenes.Where(x => x.Enabled))
        {
            var posts = await _votesRepository.GetTopForWeek(10);
        
            if (posts.Count > 0)
                await PostWeeklyTopForScene(scene, posts);
        }
        
        return Unit.Value;
    }

    private async Task PostWeeklyTopForScene(Scene scene, IReadOnlyCollection<PostedMedia> postedMedia)
    {
        var topMedia = postedMedia.Where(x => x.FileType != MediaType.Unknown).ToList();
        var caption = GetCaption(topMedia);

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

        await _telegramBotClient.SendMediaGroupAsync(scene.ChatId, fileIds);
    }

    private static string GetCaption(List<PostedMedia> topMedia)
    {
        var mediaLinks = topMedia.Select((x, i) =>
            $"<a href=\"https://t.me/c/{x.ChatId}/{x.MessageId}\">{i + 1}.</a>" +
            $" <a href=\"{GetLinkToPost(x.ImageSource, x.PostId)}\">{x.ImageSource.ToString().ToLower()}</a>");

        var caption = "💙 Популярное за неделю ~ " + string.Join(" | ", mediaLinks);
        return caption;
    }

    private static InputMediaBase CreateMedia(MediaType fileType, string fileId)
        => fileType == MediaType.Photo
            ? new InputMediaPhoto(fileId)
            : new InputMediaVideo(fileId);

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
