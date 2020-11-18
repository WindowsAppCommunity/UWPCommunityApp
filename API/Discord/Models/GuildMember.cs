using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.Models
{
    public class GuildMember
    {
        [JsonProperty(PropertyName = "user")]
        public User User { get; set; }

        [JsonProperty(PropertyName = "nick")]
        public string Nickname { get; set; }

        [JsonProperty(PropertyName = "roles")]
        public List<string> Roles { get; set; }

        /// <summary>
        /// ISO8601 timestamp when the user joined the guild
        /// </summary>
        [JsonProperty(PropertyName = "joined_at")]
        public string JoinedAt { get; set; }

        /// <summary>
        /// ISO8601 timestamp when the user started boosting the guild, if at all
        /// </summary>
        [JsonProperty(PropertyName = "boosted_at")]
        public string BoostedAt { get; set; }

        [JsonProperty(PropertyName = "deaf")]
        public bool IsDeaf { get; set; }

        [JsonProperty(PropertyName = "mute")]
        public bool IsMute { get; set; }
    }
}
