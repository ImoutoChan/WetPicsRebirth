using Microsoft.EntityFrameworkCore;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Data.Repositories.Abstract;

namespace WetPicsRebirth.Data.Repositories;

internal class UserAccountsRepository : IUserAccountsRepository
{
    private readonly WetPicsRebirthDbContext _context;

    public UserAccountsRepository(WetPicsRebirthDbContext context) => _context = context;

    public async Task<UserAccount> Set(long userId, ImageSource source, string login, string apikey)
    {
        var found = await _context.UserAccounts.FirstOrDefaultAsync(x => x.UserId == userId && x.Source == source);

        if (found == null)
        {
            found = new UserAccount(userId, source, login, apikey);
            _context.UserAccounts.Add(found);
        }

        found.Login = login;
        found.ApiKey = apikey;

        await _context.SaveChangesAsync();

        return found;
    }

    public Task<UserAccount?> Get(long userId, ImageSource source)
        => _context.UserAccounts.FirstOrDefaultAsync(x => x.UserId == userId && x.Source == source);
}