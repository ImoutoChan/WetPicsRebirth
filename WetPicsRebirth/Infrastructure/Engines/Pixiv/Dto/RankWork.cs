using Newtonsoft.Json;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv.Dto
{
    public class RankWork
    {

        [JsonProperty("rank")]
        public int? Rank { get; set; }

        [JsonProperty("previous_rank")]
        public int? PreviousRank { get; set; }

        [JsonProperty("work")]
        public Work Work { get; set; } = default!;
    }
}
