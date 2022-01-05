using System.Linq;
using System.Threading.Tasks;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly WetPicsRebirthDbContext _context;

    public UsersRepository(WetPicsRebirthDbContext context)
    {
        _context = context;
    }

    public async Task AddOrUpdate(User user)
    {
        var existing = _context.Users.FirstOrDefault(x => x.Id == user.Id);

        if (existing == null)
        {
            _context.Users.Add(user);
        }
        else
        {
            existing.Username = user.Username;
            existing.FirstName = user.FirstName;
            existing.LastName = user.LastName;
        }

        await _context.SaveChangesAsync();
    }
}