using WetPicsRebirth.Infrastructure.Models;

namespace WetPicsRebirth.Services;

public interface IModerationService
{
    Task<bool> CheckPost(Post post);
}
