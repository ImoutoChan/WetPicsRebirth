using System.Collections.Generic;
using System.Threading.Tasks;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Infrastructure.Models;

namespace WetPicsRebirth.Infrastructure
{
    public class PopularListLoader : IPopularListLoader
    {
        private readonly IEngineFactory _engineFactory;

        public PopularListLoader(IEngineFactory engineFactory)
        {
            _engineFactory = engineFactory;
        }

        public Task<IReadOnlyCollection<PostHeader>> Load(ImageSource source, string options)
        {
            return _engineFactory.Get(source).LoadPopularList(options);
        }

        public Task<Post> LoadPost(ImageSource source, PostHeader header)
        {
            return _engineFactory.Get(source).LoadPost(header);
        }

        public string CreateCaption(ImageSource source, string options, Post post)
        {
            return _engineFactory.Get(source).CreateCaption(source, options, post);
        }
    }
}
