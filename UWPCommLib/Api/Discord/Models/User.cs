using Newtonsoft.Json;

namespace UWPCommLib.Api.Discord.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public string DiscordId { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "avatar")]
        public string Avatar { get; set; }

        [JsonProperty(PropertyName = "discriminator")]
        public string Discriminator { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "verified")]
        public bool IsVerified { get; set; }

        [JsonProperty(PropertyName = "locale")]
        public string Locale { get; set; }
    }
}
