using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Quartz;
using WetPicsRebirth.Commands.ServiceCommands.Posting;

namespace WetPicsRebirth.Jobs;

internal sealed class PostingJob : IJob
{
    private readonly IMediator _mediator;
    private static readonly SemaphoreSlim Locker = new SemaphoreSlim(1);

    public PostingJob(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        if (!await Locker.WaitAsync(0))
            return;

        try
        {
            await _mediator.Send(new PostNext());
        }
        finally
        {
            Locker.Release();
        }
    }
}