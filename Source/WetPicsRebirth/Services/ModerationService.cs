using WetPicsRebirth.Commands.ServiceCommands;
using WetPicsRebirth.Infrastructure.Models;

namespace WetPicsRebirth.Services;

internal class ModerationService : IModerationService
{
    private readonly IConfiguration _configuration;
    private readonly IMediator _mediator;

    public ModerationService(IConfiguration configuration, IMediator mediator)
    {
        _configuration = configuration;
        _mediator = mediator;
    }

    public async Task<bool> CheckPost(Post post)
    {
        var moderatorId = _configuration.GetValue<long>("ModeratorId");

        return await _mediator.Send(new CheckPostQuery(post, moderatorId));
    }
}
