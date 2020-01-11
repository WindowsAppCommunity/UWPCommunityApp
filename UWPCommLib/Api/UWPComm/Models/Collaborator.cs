using Newtonsoft.Json;

namespace UWPCommLib.Api.UWPComm.Models
{
    public class Collaborator
    {
        [JsonProperty(PropertyName = "discordId")]
        public string DiscordId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "role")]
        public string Role { get; set; }

        [JsonProperty(PropertyName = "isOwner")]
        public bool IsOwner { get; set; }
    }
}
