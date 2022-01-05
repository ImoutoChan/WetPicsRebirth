namespace WetPicsRebirth.Infrastructure.Models;

public record Post(PostHeader PostHeader, string Url, Stream File, long FileSize);