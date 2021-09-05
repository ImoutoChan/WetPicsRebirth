using System;
using System.Collections.Generic;

namespace WetPicsRebirth.Data.Entities
{
    public class PostedMedia : IEntityBase
    {
        public PostedMedia(
            Guid id,
            long chatId,
            int messageId,
            string fileId,
            ImageSource imageSource,
            int postId)
        {
            Id = id;
            ChatId = chatId;
            MessageId = messageId;
            FileId = fileId;
            ImageSource = imageSource;
            PostId = postId;
        }

        public Guid Id { get; private set; }

        public long ChatId { get; private set; }

        public int MessageId { get; private set; }

        public string FileId { get; private set; }

        public ImageSource ImageSource { get; private set; }

        public int PostId { get; private set; }

        public DateTimeOffset AddedDate { get; set; }

        public DateTimeOffset ModifiedDate { get; set; }


        public IReadOnlyCollection<Vote>? Votes { get; set; }
    }
}
