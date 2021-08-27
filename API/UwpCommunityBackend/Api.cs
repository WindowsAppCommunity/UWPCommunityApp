using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UwpCommunityBackend.Models;

namespace UwpCommunityBackend
{
    public static class Api
    {
        public const string WEB_BASE_URL = "https://uwpcommunity-site-backend.herokuapp.com";
        public const string LOCAL_BASE_URL = "http://localhost:5000";

        public const string GITHUB_RAW_BASE_URL = "https://raw.githubusercontent.com/UWPCommunity/uwpcommunity.github.io/gh-pages-dev/";

        public static string BaseUrl { get; set; } = WEB_BASE_URL;

        public static string Token { get; set; }

        public static IFlurlRequest GetBase(bool authenticate)
        {
            var request = BaseUrl.WithTimeout(10);
            if (authenticate)
                request = request.WithOAuthBearerToken(Token);
            return request;
        }

        #region /projects/
        /// <summary>
        /// Gets the complete list of registered projects
        /// </summary>
        public static async Task<List<Project>> GetProjects(bool authenticate = false)
        {
            return await GetBase(authenticate).AppendPathSegment("projects")
                .GetJsonAsync<List<Project>>();
        }

        /// <summary>
        /// Gets the complete list of projects that are registered for the specified Launch year
        /// </summary>
        public static async Task<LaunchProjects> GetLaunchProjects(int year, bool authenticate = false)
        {
            return await GetBase(authenticate).AppendPathSegments("projects", "launch", year.ToString())
                .GetJsonAsync<LaunchProjects>();
        }

        /// <summary>
        /// Gets the project with the specified ID
        /// </summary>
        public static async Task<Project> GetProject(int projectId, bool authenticate = false)
        {
            return await GetBase(authenticate).AppendPathSegments("projects", "id", projectId.ToString())
                .GetJsonAsync<Project>();
        }

        /// <summary>
        /// Gets the list of collaborators for the specified project
        /// </summary>
        public static async Task<List<Collaborator>> GetProjectCollaborators(int projectId, bool authenticate = false)
        {
            return await GetBase(true).AppendPathSegments("projects", "collaborators")
                .SetQueryParam(nameof(projectId), projectId.ToString())
                .GetJsonAsync<List<Collaborator>>();
        }

        /// <summary>
        /// Registers a project with the given details
        /// </summary>
        public static async Task<HttpResponseMessage> PostProject(Project info)
        {
            return await GetBase(true).AppendPathSegments("projects")
                .SendUrlEncodedAsync(HttpMethod.Post, info);
        }

        /// <summary>
        /// Updates the project with the given details
        /// </summary>
        public static async Task<HttpResponseMessage> PutProject(string appName, Project info)
        {
            return await GetBase(true).AppendPathSegments("projects")
                .SetQueryParam(nameof(appName), appName)
                .SendJsonAsync(HttpMethod.Put, info);
        }

        /// <summary>
        /// Deletes the project that matches the app name the closest
        /// </summary>
        public static async Task<HttpResponseMessage> DeleteProject(DeleteProjectRequest info)
        {
            return await GetBase(true).AppendPathSegments("projects")
                .SendUrlEncodedAsync(HttpMethod.Delete, info);
        }

        /// <summary>
        /// Gets the list of projects with the tag specified by the <paramref name="tagId"/>
        /// </summary>
        public static async Task<List<Project>> GetTags(int tagId, bool authenticate = false)
        {
            return await GetBase(authenticate).AppendPathSegments("projects", "tags")
                .SetQueryParam(nameof(tagId), tagId)
                .GetJsonAsync<List<Project>>();
        }

        /// <summary>
        /// Gets the list of projects with the tag specified by the <paramref name="tagName"/>
        /// </summary>
        public static async Task<List<Project>> GetTags(string tagName, bool authenticate = false)
        {
            return await GetBase(authenticate).AppendPathSegments("projects", "tags")
                .SetQueryParam(nameof(tagName), tagName)
                .GetJsonAsync<List<Project>>();
        }
        #endregion

        #region /user/
        /// <summary>
        /// Gets the user's registered projects (requires authentication)
        /// </summary>
        /// <param name="userId">The Discord ID of the user</param>
        public static async Task<List<Project>> GetUserProjects(string userId)
        {
            return await GetBase(true).AppendPathSegments("user", userId, "projects")
                .GetJsonAsync<List<Project>>();
        }

        /// <summary>
        /// Gets the user's profile information
        /// </summary>
        /// <param name="userId">The Discord ID of the user</param>
        public static async Task<Collaborator> GetUser(string userId, bool authenticate = false)
        {
            return await GetBase(authenticate).AppendPathSegments("user", userId)
                .GetJsonAsync<Collaborator>();
        }

        /// <summary>
        /// Sets the user's profile information
        /// </summary>
        public static async Task<HttpResponseMessage> SetUser(Dictionary<string, string> newInfo)
        {
            return await GetBase(true).AppendPathSegments("user")
                .SendUrlEncodedAsync(HttpMethod.Put, newInfo);
        }

        /// <summary>
        /// Creates a user with the specified profile information
        /// </summary>
        public static async Task<HttpResponseMessage> PostUser(Dictionary<string, string> info)
        {
            return await GetBase(true).AppendPathSegments("user")
                .SendUrlEncodedAsync(HttpMethod.Post, info);
        }
        #endregion

        #region /bot/
        /// <summary>
        /// Gets the user's profile information from Discord
        /// </summary>
        public static async Task<Discord.Models.User> GetDiscordUser(string userId)
        {
            return await GetBase(true).AppendPathSegments("bot", "user", userId)
                .GetJsonAsync<Discord.Models.User>();
        }

        /// <summary>
        /// Gets the user's roles in the UWP Community Discord server
        /// </summary>
        public static async Task<List<Discord.Models.Role>> GetDiscordUserRoles(string userId)
        {
            return await GetBase(true).AppendPathSegments("bot", "user", userId, "roles")
                .GetJsonAsync<List<Discord.Models.Role>>();
        }
        #endregion

        #region /signin/
        public static async Task<HttpResponseMessage> Redirect(string code)
        {
            return await BaseUrl.AppendPathSegments("signin", "redirect")
                .SetQueryParam(nameof(code), code)
                .GetAsync();
        }

        public static async Task<Tuple<Discord.Models.TokenResponse, string>> UseRefreshToken(string refreshToken)
        {
            var httpResponse = await BaseUrl.AppendPathSegments("signin", "refresh")
                .SetQueryParam(nameof(refreshToken), refreshToken)
                .GetAsync();

            // TODO: Use response.IsSuccessStatusCode
            // At the moment, the API returns 200 OK even if there was an error.
            var responseString = await httpResponse.Content.ReadAsStringAsync();

            // The API returns an escaped string containing a JSON object. The following two
            // lines unescape the string and remove the enclosing qoutes.
            responseString = System.Text.RegularExpressions.Regex.Unescape(responseString);
            responseString = responseString.Substring(1, responseString.Length - 2);
            if (responseString.Contains("error"))
            {
                //JsonConvert.DeserializeObject<TokenResponseError>(responseString)
                return new Tuple<Discord.Models.TokenResponse, string>(null, "An error occurred, perhaps an invalid refresh token?");
            }
            else
            {
                return new Tuple<Discord.Models.TokenResponse, string>(
                    JsonConvert.DeserializeObject<Discord.Models.TokenResponse>(responseString), null);
            }
        }
        #endregion

        public static async Task<CardInfoResponse> GetCard(string name)
        {
            return await GITHUB_RAW_BASE_URL.AppendPathSegments("src", "assets", "views", name + ".json")
                .GetJsonAsync<CardInfoResponse>();
        }

        public static readonly Dictionary<string, string> SpecialRoles = new Dictionary<string, string>()
        {
            { "Developer", "746853910974562394" },
            { "Designer", "746853909783380029" },
            { "LlamaSqaud", "746853934571978802" }
        };
    }
}
