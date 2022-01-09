using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Infrastructure.Models;

namespace WetPicsRebirth.Infrastructure;

public interface IPopularListLoaderEngine
{
    public Task<IReadOnlyCollection<PostHeader>> LoadPopularList(string options);

    public Task<LoadedPost> LoadPost(PostHeader postHeader);

    string CreateCaption(ImageSource source, string options, Post post);
}
