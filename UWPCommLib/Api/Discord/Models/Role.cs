using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UWPCommLib.Api.Discord.Models
{
    public class Role
    {
        [JsonProperty(PropertyName = "deleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Integer representation of the HEX color code
        /// </summary>
        [JsonProperty(PropertyName = "color")]
        public int Color { get; set; }

        /// <summary>
        /// If this role is pinned in the user listing
        /// </summary>
        [JsonProperty(PropertyName = "hoist")]
        public bool IsHoisted { get; set; }

        [JsonProperty(PropertyName = "position")]
        public int Position { get; set; }

        [JsonProperty(PropertyName = "permissions")]
        public int Permissions { get; set; }

        [JsonProperty(PropertyName = "managed")]
        public bool Managed { get; set; }

        [JsonProperty(PropertyName = "mentionable")]
        public bool Mentionable { get; set; }
    }
}
