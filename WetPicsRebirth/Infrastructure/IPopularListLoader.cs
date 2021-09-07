using System.Collections.Generic;
using System.Threading.Tasks;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Infrastructure.Models;

namespace WetPicsRebirth.Infrastructure
{
    /// <summary>
    /// Loading tops from boorus and pixiv
    /// </summary>
    public interface IPopularListLoader
    {
        public Task<IReadOnlyCollection<PostHeader>> Load(ImageSource source, string options);
    }
}
