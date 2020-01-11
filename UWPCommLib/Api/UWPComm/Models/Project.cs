using Newtonsoft.Json;
using System.Collections.Generic;

namespace UWPCommLib.Api.UWPComm.Models
{
    public class Project
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "appName")]
        public string AppName { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "isPrivate")]
        public bool IsPrivate { get; set; }

        [JsonProperty(PropertyName = "downloadLink")]
        public string DownloadLink { get; set; }

        [JsonProperty(PropertyName = "githubLink")]
        public string GitHubLink { get; set; }

        [JsonProperty(PropertyName = "externalLink")]
        public string ExternalLink { get; set; }

        [JsonProperty(PropertyName = "collaborators")]
        public List<Collaborator> Collaborators { get; set; }

        [JsonProperty(PropertyName = "launchYear")]
        public short LaunchYear { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public string CreatedAt { get; set; }

        [JsonProperty(PropertyName = "updatedAt")]
        public string UpdatedAt { get; set; }

        [JsonProperty(PropertyName = "awaitingLaunchApproval")]
        public bool? IsAwaitingLaunchApproval { get; set; }

        [JsonProperty(PropertyName = "needsManualReview")]
        public bool NeedsManualReview { get; set; }

        [JsonProperty(PropertyName = "heroImage")]
        public string HeroImage { get; set; }

        [JsonProperty(PropertyName = "lookingForRoles")]
        public bool? IsLookingForRoles { get; set; }

        public System.Uri HeroImageUri {
            get {
                return new System.Uri(HeroImage);
            }
        }
    }
}
