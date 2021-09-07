using Newtonsoft.Json;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv.Dto
{
    public class UserStats
    {

        [JsonProperty("works")]
        public int? Works { get; set; }

        [JsonProperty("favorites")]
        public int? Favorites { get; set; }

        [JsonProperty("following")]
        public int? Following { get; set; }

        [JsonProperty("friends")]
        public int? Friends { get; set; }
    }
}