using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Data.Repositories;
using WetPicsRebirth.Infrastructure;

namespace WetPicsRebirth.Services.UserAccounts;

internal class LikesToFavoritesTranslator : ILikesToFavoritesTranslator
{
    private readonly IImageSourceApi _imageSourceApi;
    private readonly ILogger<LikesToFavoritesTranslator> _logger;
    private readonly IPostedMediaRepository _postedMediaRepository;
    private readonly IUserAccountsRepository _userAccountsRepository;

    public LikesToFavoritesTranslator(
        IUserAccountsRepository userAccountsRepository,
        IPostedMediaRepository postedMediaRepository,
        ILogger<LikesToFavoritesTranslator> logger,
        IImageSourceApi imageSourceApi)
    {
        _userAccountsRepository = userAccountsRepository;
        _postedMediaRepository = postedMediaRepository;
        _logger = logger;
        _imageSourceApi = imageSourceApi;
    }

    public async Task Translate(Vote vote)
    {
        var media = await _postedMediaRepository.Get(vote.ChatId, vote.MessageId);
        if (media == null)
        {
            _logger.LogWarning(
                "Unable to find a media to translate like {ChatId} {MessageId}",
                vote.ChatId,
                vote.MessageId);
            return;
        }

        var account = await _userAccountsRepository.Get(vote.UserId, media.ImageSource);
        if (account == null)
        {
            _logger.LogInformation(
                "No account to translate like {ChatId} {MessageId} {UserId}",
                vote.ChatId,
                vote.MessageId,
                vote.UserId);
            return;
        }

        await _imageSourceApi.FavoritePost(account, media.PostId);
    }
}
