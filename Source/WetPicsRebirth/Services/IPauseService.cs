using WetPicsRebirth.Commands.ServiceCommands.Posting;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Services;

public interface IPauseService
{
    bool IsPaused(Scene scene);

    void Pause(Scene scene, TopType topType);
}
