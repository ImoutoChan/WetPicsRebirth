using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using WetPicsRebirth.EntryPoint.Service;

namespace WetPicsRebirth.EntryPoint;

[ApiController]
[Route("api/wetpicsrebirth/update")] // 41461
public class UpdateController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public UpdateController(INotificationService notificationService)
        => _notificationService = notificationService;

    [HttpPost]
    public Task Post([FromBody] Update update) => _notificationService.NotifyAsync(update);
}