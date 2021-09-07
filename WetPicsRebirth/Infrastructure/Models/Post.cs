namespace WetPicsRebirth.Infrastructure.Models
{
    public record Post(PostHeader PostHeader, string Url, byte[] File);
}
