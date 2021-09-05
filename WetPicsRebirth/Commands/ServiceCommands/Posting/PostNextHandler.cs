using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace WetPicsRebirth.Commands.ServiceCommands.Posting
{
    public class PostNextHandler : IRequestHandler<PostNext>
    {
        public Task<Unit> Handle(PostNext request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
