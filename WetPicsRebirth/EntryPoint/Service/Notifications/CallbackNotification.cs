using Telegram.Bot.Types;

namespace WetPicsRebirth.EntryPoint.Service.Notifications;

public class CallbackNotification : INotification
{
    public CallbackNotification(CallbackQuery callbackQuery)
    {
        CallbackQuery = callbackQuery;
    }

    public CallbackQuery CallbackQuery { get; }
}