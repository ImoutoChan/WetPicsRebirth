using Microsoft.EntityFrameworkCore;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories;

internal class ActressesRepository : IActressesRepository
{
    private readonly WetPicsRebirthDbContext _context;

    public ActressesRepository(WetPicsRebirthDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Actress>> GetForChat(long targetChatId)
    {
        return await _context.Actresses.Where(x => x.ChatId == targetChatId).ToListAsync();
    }

    public async Task Add(long targetChatId, ImageSource source, string options)
    {
        var newActress = new Actress(targetChatId, source, options);

        _context.Actresses.Add(newActress);
        await _context.SaveChangesAsync();
    }

    public async Task Remove(Guid id)
    {
        var actress = await _context.Actresses.FirstOrDefaultAsync(x => x.Id == id);

        if (actress != null)
        {
            _context.Actresses.Remove(actress);
            await _context.SaveChangesAsync();
        }
    }
}