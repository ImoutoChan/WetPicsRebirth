using MediatR;
using WetPicsRebirth.EntryPoint.Service;

namespace WetPicsRebirth.Commands.Abstract
{
    public interface ICallbackHandler : INotificationHandler<MessageNotification>
    {
    }
}