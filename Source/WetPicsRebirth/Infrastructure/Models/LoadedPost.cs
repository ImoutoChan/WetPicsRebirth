namespace WetPicsRebirth.Infrastructure.Models;

public record LoadedPost(Post Post, bool RequireModeration = false);
