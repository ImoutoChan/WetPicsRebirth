using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Infrastructure;

public interface IImageSourceApi
{
    public Task FavoritePost(UserAccount account, int postId);
}
