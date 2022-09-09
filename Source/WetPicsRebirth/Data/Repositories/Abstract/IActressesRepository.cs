using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories.Abstract;

public interface IActressesRepository
{
    Task<IReadOnlyCollection<Actress>> GetForChat(long targetChatId);

    Task Add(long targetChatId, ImageSource source, string options);

    Task Remove(Guid id);
}
