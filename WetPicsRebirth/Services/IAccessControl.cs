using System.Threading.Tasks;
using MediatR;

namespace WetPicsRebirth.Services
{
    public interface IAccessControl
    {
        Task<bool> CheckAccess(INotification notification);
    }
}
