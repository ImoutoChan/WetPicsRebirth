using System.Collections.Generic;
using Newtonsoft.Json;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv.Dto
{
    public class Metadata
    {
        [JsonProperty("pages")]
        public IList<Page> Pages { get; set; } = default!;
    }
}
