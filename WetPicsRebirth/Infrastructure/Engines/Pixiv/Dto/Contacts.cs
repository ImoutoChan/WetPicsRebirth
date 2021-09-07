using Newtonsoft.Json;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv.Dto
{
    public class Contacts
    {
        [JsonProperty("twitter")]
        public string Twitter { get; set; } = default!;
    }
}
