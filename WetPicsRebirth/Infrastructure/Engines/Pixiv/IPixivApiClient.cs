using System.Collections.Generic;
using System.Threading.Tasks;
using WetPicsRebirth.Infrastructure.Engines.Pixiv.Models;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv
{
    public interface IPixivApiClient
    {
        Task<IReadOnlyCollection<PixivPostHeader>> LoadTop(PixivTopType topType, int page = 1, int count = 100);

        Task<byte[]> DownloadImage(string imageUrl);
    }
}
