using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;

namespace WetPicsRebirth.Data.Entities;

public class Scene : IEntityBase
{
    public Scene(
        long chatId,
        int minutesInterval,
        Instant? lastPostedTime,
        bool enabled)
    {
        ChatId = chatId;
        MinutesInterval = minutesInterval;
        LastPostedTime = lastPostedTime;
        Enabled = enabled;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long ChatId { get; private set; }

    public int MinutesInterval { get; set; }

    public Instant? LastPostedTime { get; set; }

    public bool Enabled { get; set; }

    public Instant AddedDate { get; set; }

    public Instant ModifiedDate { get; set; }


    public IReadOnlyCollection<Actress>? Actresses { get; set; }
}
