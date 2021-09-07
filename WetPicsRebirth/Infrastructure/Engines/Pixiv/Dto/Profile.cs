using Newtonsoft.Json;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv.Dto
{
    public class Profile
    {
        [JsonProperty("contacts")]
        public Contacts Contacts { get; set; } = default!;

        [JsonProperty("workspace")]
        public Workspace Workspace { get; set; } = default!;

        [JsonProperty("job")]
        public string Job { get; set; } = default!;

        [JsonProperty("introduction")]
        public string Introduction { get; set; } = default!;

        [JsonProperty("location")]
        public string Location { get; set; } = default!;

        [JsonProperty("gender")]
        public string Gender { get; set; } = default!;

        [JsonProperty("tags")]
        public object Tags { get; set; } = default!;

        [JsonProperty("homepage")]
        public string Homepage { get; set; } = default!;

        [JsonProperty("birth_date")]
        public string BirthDate { get; set; } = default!;

        [JsonProperty("blood_type")]
        public string BloodType { get; set; } = default!;
    }
}
