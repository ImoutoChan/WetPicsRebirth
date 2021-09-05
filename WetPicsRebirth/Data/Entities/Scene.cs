using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WetPicsRebirth.Data.Entities
{
    public class Scene : IEntityBase
    {
        public Scene(
            long chatId,
            int minutesInterval,
            DateTimeOffset? lastPostedTime,
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

        public DateTimeOffset? LastPostedTime { get; set; }

        public bool Enabled { get; set; }

        public DateTimeOffset AddedDate { get; set; }

        public DateTimeOffset ModifiedDate { get; set; }


        public IReadOnlyCollection<Actress>? Actresses { get; set; }
    }
}