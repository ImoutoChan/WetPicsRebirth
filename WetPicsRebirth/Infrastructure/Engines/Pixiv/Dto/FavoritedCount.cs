using Newtonsoft.Json;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv.Dto
{
    public class FavoritedCount
    {
        [JsonProperty("public")]
        public int? Public { get; set; }

        [JsonProperty("private")]
        public int? Private { get; set; }
    }
}