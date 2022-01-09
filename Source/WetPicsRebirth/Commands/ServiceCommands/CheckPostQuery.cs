using WetPicsRebirth.Infrastructure.Models;

namespace WetPicsRebirth.Commands.ServiceCommands;

public record CheckPostQuery(Post Post, long ModeratorId) : IRequest<bool>;
