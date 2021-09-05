using System.Threading.Tasks;
using Quartz;

namespace WetPicsRebirth.Jobs
{
    internal sealed class PostingJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            //todo throw new System.NotImplementedException();
            return Task.CompletedTask;
        }
    }
}
