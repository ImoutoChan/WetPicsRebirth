using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories.Abstract;

public interface IPostedMediaRepository
{
    Task Add(
        long chatId,
        int messageId,
        string fileId,
        MediaType fileType,
        ImageSource imageSource,
        int postId,
        string postHash);

    Task<int?> GetFirstNew(
        long chatId,
        ImageSource imageSource,
        List<(int Id, string? Md5Hash)> postIdsWithHashes);

    Task<PostedMedia?> Get(long chatId, int messageId);
}
