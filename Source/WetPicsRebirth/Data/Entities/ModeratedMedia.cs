namespace WetPicsRebirth.Data.Entities;

public class ModeratedMedia : IEntityBase
{
    public ModeratedMedia(Guid id, int postId, string hash, int messageId, bool isApproved)
    {
        Id = id;
        PostId = postId;
        Hash = hash;
        MessageId = messageId;
        IsApproved = isApproved;
    }

    public Guid Id { get; private set; }

    public int PostId { get; private set; }

    public string Hash { get; private set; }

    public int MessageId { get; private set; }

    public bool IsApproved { get; set; }

    public Instant AddedDate { get; set; }

    public Instant ModifiedDate { get; set; }
}
