using Newtonsoft.Json;

namespace Discord.Models
{
    public class Guild
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; set; }

        [JsonProperty(PropertyName = "permissions")]
        public int Permissions { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public bool IsOwner { get; set; }
    }
}
