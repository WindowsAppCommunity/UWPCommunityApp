using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace UwpCommunityBackend.Models
{
    public class Collaborator : UserBase
    {
        [JsonProperty(PropertyName = "role")]
        public RoleType? Role { get; set; }

        [JsonProperty(PropertyName = "isOwner")]
        public bool IsOwner { get; set; }

        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public enum RoleType
        {
            Developer,

            Translator,

            [EnumMember(Value = "Beta Tester")]
            BetaTester,

            Lead,

            Advocate,

            Support,

            Patreon,

            Other
        }
    }
}
