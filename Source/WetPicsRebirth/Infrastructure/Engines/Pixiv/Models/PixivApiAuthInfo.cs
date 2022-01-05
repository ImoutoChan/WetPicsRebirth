namespace WetPicsRebirth.Infrastructure.Engines.Pixiv.Models;

public record PixivApiAuthInfo(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);