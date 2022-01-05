using System;
using System.Collections.Generic;
using NodaTime;

namespace WetPicsRebirth.Data.Entities;

public class PostedMedia : IEntityBase
{
    public PostedMedia(
        Guid id,
        long chatId,
        int messageId,
        string fileId,
        ImageSource imageSource,
        int postId,
        string postHash)
    {
        Id = id;
        ChatId = chatId;
        MessageId = messageId;
        FileId = fileId;
        ImageSource = imageSource;
        PostId = postId;
        PostHash = postHash;
    }

    public Guid Id { get; private set; }

    public long ChatId { get; private set; }

    public int MessageId { get; private set; }

    public string FileId { get; private set; }

    public ImageSource ImageSource { get; private set; }

    public int PostId { get; private set; }

    public string PostHash { get; private set; }

    public Instant AddedDate { get; set; }

    public Instant ModifiedDate { get; set; }


    public IReadOnlyCollection<Vote>? Votes { get; set; }
}