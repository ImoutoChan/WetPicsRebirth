using Newtonsoft.Json;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv.Dto
{
    public class Page
    {
        [JsonProperty("image_urls")]
        public ImageUrls ImageUrls { get; set; } = default!;
    }
}
