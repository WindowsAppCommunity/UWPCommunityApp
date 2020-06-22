using Newtonsoft.Json;

namespace UWPCommLib.Api.UWPComm.Models
{
    public class Collaborator : UserBase
    {
        [JsonProperty(PropertyName = "role")]
        public RoleType Role { get; set; }

        [JsonProperty(PropertyName = "isOwner")]
        public bool IsOwner { get; set; }

        public enum RoleType
        {
            Developer,
            Translator,
            BetaTester,
            Other
        }
    }
}
