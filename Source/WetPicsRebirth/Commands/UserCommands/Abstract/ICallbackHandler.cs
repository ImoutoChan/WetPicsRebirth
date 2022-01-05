using WetPicsRebirth.EntryPoint.Service.Notifications;

namespace WetPicsRebirth.Commands.UserCommands.Abstract;

public interface ICallbackHandler : INotificationHandler<CallbackNotification>
{
}