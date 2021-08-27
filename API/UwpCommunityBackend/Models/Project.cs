using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UwpCommunityBackend.Models
{
    public class Project
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("collaborators")]
        public List<Collaborator> Collaborators { get; set; }
        public bool IsCollaboratorsAvailable => Collaborators != null && Collaborators.Count > 0;

        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }

        [JsonProperty("needsManualReview")]
        public bool NeedsManualReview { get; set; }

        [JsonProperty("appName")]
        public string AppName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

        [JsonProperty("downloadLink")]
        public string DownloadLink { get; set; }

        [JsonProperty("githubLink")]
        public string GitHubLink { get; set; }

        [JsonProperty("externalLink")]
        public string ExternalLink { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("awaitingLaunchApproval")]
        public bool? IsAwaitingLaunchApproval { get; set; }

        [JsonProperty("heroImage")]
        public string HeroImage { get; set; } = "https://uwpcommunity.com/assets/img/LaunchHero.jpg";

        [JsonProperty("appIcon")]
        public string AppIcon { get; set; } = "https://uwpcommunity.com/assets/img/LaunchHero.jpg";

        [JsonProperty("lookingForRoles")]
        public bool? IsLookingForRoles { get; set; }

        [JsonProperty("images")]
        public List<object> Images { get; set; }

        [JsonProperty("tags")]
        public List<Tag> Tags { get; set; }

        [JsonProperty("accentColor")]
        public string AccentColor { get; set; }

        /// <summary>
        /// The role the user had in this project. Only for use with POST /projects/
        /// </summary>
        [JsonProperty("role")]
        public string Role { get; set; } = "Developer";

        [JsonIgnore]
        public Uri HeroImageUri => new Uri(HeroImageSafe);

        [JsonIgnore]
        /// <summary>
        /// A duplicate of HeroImage, but returns a dummy image if null
        /// </summary>
        public string HeroImageSafe => HeroImage ?? "https://uwpcommunity.com/assets/img/LaunchHero.png";

        [JsonIgnore]
        public Collaborator Owner => Collaborators?.First(c => c.IsOwner);

        public enum ProjectCategory
        {
            BooksAndReference,
            Business,
            DeveloperTools,
            Education,
            Entertainment,
            FoodAndDining,
            GovernmentAndPolitics,
            KidsAndFamily,
            Lifestyle,
            Medical,
            MultimediaDesign,
            Music,
            NavigationAndMaps,
            NewsAndWeather,
            PersonalFinance,
            Personalization,
            PhotoAndVideo,
            Productivity,
            Security,
            Shopping,
            Social,
            Sports,
            Travel,
            UtilitiesAndTools
        }
        public static string GetCategoryTitle(ProjectCategory category)
        {
            string catStr = category.ToString().Replace("And", "&");
            string name = string.Empty;
            for (int i = 0; i < catStr.Length; i++)
            {
                char ch = catStr[i];
                if (i == 0 || char.IsLower(ch))
                {
                    name += ch;
                }
                else if (char.IsUpper(ch) || ch == '&')
                {
                    name += " " + char.ToLower(ch);
                }
            }
            return name;
        }
        public static ProjectCategory GetCategoryFromTitle(string name)
        {
            string catStr = name.ToString()
                .Replace(" ", string.Empty).Replace("&", "And");
            return (ProjectCategory)Enum.Parse(typeof(ProjectCategory), catStr, true);
        }

        /// <summary>
        /// Checks if the user is the owner of this project
        /// </summary>
        public bool IsOwner(string userId)
        {
            var owner = Collaborators.Find(c => c.IsOwner == true);
            return owner.DiscordId == userId;
        }

        /// <summary>
        /// Checks if the user is a developer for this project
        /// </summary>
        public bool IsDeveloper(string userId)
        {
            var user = Collaborators.Find(c => c.DiscordId == userId);
            return user.Role == Collaborator.RoleType.Developer;
        }
        /// <summary>
        /// Checks if the user is a translator for this project
        /// </summary>
        public bool IsTranslator(string userId)
        {
            var user = Collaborators.Find(c => c.DiscordId == userId);
            return user.Role == Collaborator.RoleType.Translator;
        }
        /// <summary>
        /// Checks if the user is a beta tester for this project
        /// </summary>
        public bool IsBetaTester(string userId)
        {
            var user = Collaborators.Find(c => c.DiscordId == userId);
            return user.Role == Collaborator.RoleType.BetaTester;
        }

        public IEnumerable<Collaborator> GetDevelopers()
        {
            return Collaborators.Where(c => c.Role == Collaborator.RoleType.Developer);
        }
        public IEnumerable<Collaborator> GetTranslators()
        {
            return Collaborators.Where(c => c.Role == Collaborator.RoleType.Translator);
        }
        public IEnumerable<Collaborator> GetBetaTesters()
        {
            return Collaborators.Where(c => c.Role == Collaborator.RoleType.BetaTester);
        }

        public int? GetLastLaunchYear()
        {
            int lastYear = -1;
            foreach (Tag tag in Tags.Where(t => t.Name.StartsWith("Launch ")))
            {
                if (int.TryParse(tag.Name, out int year) && year > lastYear)
                    lastYear = year;
            }
            return lastYear >= 0 ? lastYear : (int?)null;
        }

        public override string ToString()
        {
            return $"{AppName} by {Owner?.Name ?? string.Empty}: {Description}";
        }
    }

    public class NewProjectRequest
    {
        [JsonProperty("appName")]
        public string AppName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

        [JsonProperty("downloadLink")]
        public string DownloadLink { get; set; }

        [JsonProperty("githubLink")]
        public string GitHubLink { get; set; }

        [JsonProperty("externalLink")]
        public string ExternalLink { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("heroImage")]
        public string HeroImage { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }
    }

    public class DeleteProjectRequest
    {
        [JsonProperty("appName")]
        public string AppName { get; set; }

        public DeleteProjectRequest(string appName)
        {
            AppName = appName;
        }

        public static explicit operator DeleteProjectRequest(string appName)
        {
            return new DeleteProjectRequest(appName);
        }
        public static explicit operator string(DeleteProjectRequest info)
        {
            return info?.AppName;
        }

        public override string ToString()
        {
            return AppName;
        }
    }
}
