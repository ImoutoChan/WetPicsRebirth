using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using WetPicsRebirth.EntryPoint.Service.Notifications;

namespace WetPicsRebirth.Services
{
    public class AccessControl : IAccessControl
    {
        private readonly IConfiguration _configuration;

        public AccessControl(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> CheckAccess(INotification notification)
            => notification switch
            {
                MessageNotification message => await CheckAccess(message.Message.From!.Id),
                CallbackNotification => true,
                _ => false
            };

        private Task<bool> CheckAccess(long userId)
        {
            var whitelist = _configuration.GetSection("WhiteList").Get<long[]>() ?? Array.Empty<long>();

            return Task.FromResult(whitelist.Contains(userId));
        }
    }
}
