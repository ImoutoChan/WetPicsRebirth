using Quartz;
using WetPicsRebirth.Commands.ServiceCommands.Posting;

namespace WetPicsRebirth.Jobs;

internal sealed class PostYearlyTopJob : IJob
{
    private readonly IMediator _mediator;
    private static readonly SemaphoreSlim Locker = new(1);

    public PostYearlyTopJob(IMediator mediator) => _mediator = mediator;

    public async Task Execute(IJobExecutionContext context)
    {
        if (!await Locker.WaitAsync(0))
            return;

        try
        {
            await _mediator.Send(new PostTop(TopType.Yearly));
        }
        finally
        {
            Locker.Release();
        }
    }
}
