using System;
using System.Collections.Generic;

namespace WetPicsRebirth.Data.Entities
{
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

        public DateTimeOffset AddedDate { get; set; }

        public DateTimeOffset ModifiedDate { get; set; }


        public Scene? Scene { get; set; }
    }
}
