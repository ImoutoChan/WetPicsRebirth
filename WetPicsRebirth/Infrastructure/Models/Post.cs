using System.IO;

namespace WetPicsRebirth.Infrastructure.Models
{
    public record Post(PostHeader PostHeader, string Url, Stream File);
}
