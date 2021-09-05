using System;

namespace WetPicsRebirth.Data.Entities
{
    public class Vote : IEntityBase
    {
        public Vote(int userId, long chatId, int messageId, Guid postedMediaId)
        {
            UserId = userId;
            ChatId = chatId;
            MessageId = messageId;
            PostedMediaId = postedMediaId;
        }
        
        public int UserId { get; private set; }
        
        public long ChatId { get; private set; }
        
        public int MessageId { get; private set; }

        public Guid PostedMediaId { get; private set; }

        public DateTimeOffset AddedDate { get; set; }

        public DateTimeOffset ModifiedDate { get; set; }


        public User? User { get; set; }

        public PostedMedia? PostedMedia { get; set; }
    }
}