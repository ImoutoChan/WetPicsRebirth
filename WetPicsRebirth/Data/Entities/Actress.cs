using System;
using NodaTime;

namespace WetPicsRebirth.Data.Entities;

public class Actress : IEntityBase
{
    public Actress(
        long chatId,
        ImageSource imageSource,
        string options)
    {
        Id = Guid.NewGuid();
        ChatId = chatId;
        ImageSource = imageSource;
        Options = options;
    }

    public Guid Id { get; private set; }

    public long ChatId { get; private set; }

    public ImageSource ImageSource { get; private set; }

    public string Options { get; private set; }

    public Instant AddedDate { get; set; }

    public Instant ModifiedDate { get; set; }


    public Scene? Scene { get; set; }
}