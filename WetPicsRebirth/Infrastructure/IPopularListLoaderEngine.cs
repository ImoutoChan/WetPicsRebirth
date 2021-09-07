using System.Collections.Generic;
using System.Threading.Tasks;
using WetPicsRebirth.Infrastructure.Models;

namespace WetPicsRebirth.Infrastructure
{
    public interface IPopularListLoaderEngine
    {
        public Task<IReadOnlyCollection<PostHeader>> LoadPopularList(string options);

        public Task<Post> LoadPost(PostHeader postHeader);
    }
}
