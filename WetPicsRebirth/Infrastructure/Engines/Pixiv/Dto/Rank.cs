using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv.Dto
{
    public class Rank
    {

        [JsonProperty("content")]
        public string Content { get; set; } = default!;

        [JsonProperty("mode")]
        public string Mode { get; set; } = default!;

        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("works")]
        public IList<RankWork> Works { get; set; } = default!;
    }
}
