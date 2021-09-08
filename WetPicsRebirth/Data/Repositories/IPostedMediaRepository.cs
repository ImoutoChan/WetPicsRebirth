using System.Collections.Generic;
using System.Threading.Tasks;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories
{
    public interface IPostedMediaRepository
    {
        Task Add(
            long chatId,
            int messageId,
            string fileId,
            ImageSource imageSource,
            int postId);

        Task<int?> GetFirstNew(
            long chatId,
            ImageSource imageSource,
            IReadOnlyCollection<int> postIds);
    }
}
