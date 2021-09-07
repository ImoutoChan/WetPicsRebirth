using Newtonsoft.Json;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv.Dto
{
    public class ImageUrls
    {
        [JsonProperty("px_128x128")]
        public string Px128x128 { get; set; } = default!;

        [JsonProperty("small")]
        public string Small { get; set; } = default!;

        [JsonProperty("medium")]
        public string Medium { get; set; } = default!;

        [JsonProperty("large")]
        public string Large { get; set; } = default!;

        [JsonProperty("px_480mw")]
        public string Px480mw { get; set; } = default!;
    }
}
