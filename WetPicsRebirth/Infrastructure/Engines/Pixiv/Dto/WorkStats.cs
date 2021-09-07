using Newtonsoft.Json;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv.Dto
{
    public class WorkStats
    {
        [JsonProperty("scored_count")]
        public int? ScoredCount { get; set; }

        [JsonProperty("score")]
        public int? Score { get; set; }

        [JsonProperty("views_count")]
        public int? ViewsCount { get; set; }

        [JsonProperty("favorited_count")]
        public FavoritedCount FavoritedCount { get; set; } = default!;

        [JsonProperty("commented_count")]
        public int? CommentedCount { get; set; }
    }
}
