using MediatR;
using Telegram.Bot.Types;

namespace WetPicsRebirth.EntryPoint.Service
{
    public class CallbackNotification : INotification
    {
        public CallbackNotification(CallbackQuery callbackQuery)
        {
            CallbackQuery = callbackQuery;
        }

        public CallbackQuery CallbackQuery { get; }
    }
}