namespace WetPicsRebirth.Data.Repositories.Abstract;

public interface IModeratedPostsRepository
{
    Task<bool?> CheckIsApproved(int postId, string hash);

    Task Add(int postId, string hash, int sentMessageId);

    Task Set(int messageId, bool isApproved);
}
