using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace WetPicsRebirth.EntryPoint.Service
{
    public interface INotificationService
    {
        Task NotifyAsync(Update update);
    }
}