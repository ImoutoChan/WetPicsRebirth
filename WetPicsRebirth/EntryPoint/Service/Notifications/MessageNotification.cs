using MediatR;
using Telegram.Bot.Types;

namespace WetPicsRebirth.EntryPoint.Service.Notifications
{
    public class MessageNotification : INotification
    {
        public MessageNotification(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}