namespace WetPicsRebirth.Data.Entities;

public class Vote : IEntityBase
{
    public Vote(long userId, long chatId, int messageId)
    {
        UserId = userId;
        ChatId = chatId;
        MessageId = messageId;
    }

    public long UserId { get; private set; }

    public long ChatId { get; private set; }

    public int MessageId { get; private set; }

    public Instant AddedDate { get; set; }

    public Instant ModifiedDate { get; set; }


    public User? User { get; set; }

    public PostedMedia? PostedMedia { get; set; }
}