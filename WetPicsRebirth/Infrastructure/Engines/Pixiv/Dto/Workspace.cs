using Newtonsoft.Json;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv.Dto
{
    public class Workspace
    {

        [JsonProperty("computer")]
        public string Computer { get; set; } = default!;

        [JsonProperty("monitor")]
        public string Monitor { get; set; } = default!;

        [JsonProperty("software")]
        public string Software { get; set; } = default!;

        [JsonProperty("scanner")]
        public string Scanner { get; set; } = default!;

        [JsonProperty("tablet")]
        public string Tablet { get; set; } = default!;

        [JsonProperty("mouse")]
        public string Mouse { get; set; } = default!;

        [JsonProperty("printer")]
        public string Printer { get; set; } = default!;

        [JsonProperty("on_table")]
        public string OnTable { get; set; } = default!;

        [JsonProperty("music")]
        public string Music { get; set; } = default!;

        [JsonProperty("table")]
        public string Table { get; set; } = default!;

        [JsonProperty("chair")]
        public string Chair { get; set; } = default!;

        [JsonProperty("other")]
        public string Other { get; set; } = default!;

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; } = default!;

        [JsonProperty("image_urls")]
        public ImageUrls ImageUrls { get; set; } = default!;
    }
}
