using System.Security.Cryptography;
using WetPicsRebirth.Data.Repositories.Abstract;
using WetPicsRebirth.Infrastructure.ImageProcessing;
using WetPicsRebirth.Services;

namespace WetPicsRebirth.Commands.ServiceCommands;

public class CheckPostQueryHandler : IRequestHandler<CheckPostQuery, bool>
{
    private readonly IModeratedPostsRepository _moderatedPostsRepository;
    private readonly ILogger<CheckPostQueryHandler> _logger;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ITelegramPreparer _telegramPreparer;

    public CheckPostQueryHandler(
        ITelegramBotClient telegramBotClient,
        ITelegramPreparer telegramPreparer,
        IModeratedPostsRepository moderatedPostsRepository,
        ILogger<CheckPostQueryHandler> logger)
    {
        _telegramBotClient = telegramBotClient;
        _telegramPreparer = telegramPreparer;
        _moderatedPostsRepository = moderatedPostsRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(CheckPostQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return await HandleInternal(request, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Unable to send check for post {PostId}", request.Post.PostHeader.Id);
            return false;
        }
    }

    private async Task<bool> HandleInternal(CheckPostQuery request, CancellationToken cancellationToken)
    {
        var post = request.Post;
        var hash = post.PostHeader.Md5Hash ?? GetHash(post.File);

        var approved = await _moderatedPostsRepository.CheckIsApproved(post.PostHeader.Id, hash);

        if (approved != null)
            return approved.Value;

        var file = _telegramPreparer.Prepare(post.File, post.FileSize);

        var sentPost = await _telegramBotClient.SendPhotoAsync(
            request.ModeratorId,
            new(file),
            post.PostHtmlCaption,
            ParseMode.Html,
            replyMarkup: Keyboards.WithModeration,
            cancellationToken: cancellationToken);

        await _moderatedPostsRepository.Add(post.PostHeader.Id, hash, sentPost.MessageId);

        return false;
    }

    private static string GetHash(Stream file)
    {
        file.Seek(0, SeekOrigin.Begin);

        using var md5 = MD5.Create();
        var result = string.Join("", md5.ComputeHash(file).Select(x => x.ToString("X2"))).ToLowerInvariant();
        file.Seek(0, SeekOrigin.Begin);

        return result;
    }
}
