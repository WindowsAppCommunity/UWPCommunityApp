using Newtonsoft.Json;
using System.Collections.Generic;

namespace UwpCommunityBackend.Models
{
    public class Tag
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("projects")]
        public List<Project> Projects { get; set; }

        public override string ToString() => Name;
    }
}
