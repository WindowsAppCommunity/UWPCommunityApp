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

        #region /projects/
        /// <summary>
        /// Gets the complete list of registered projects
        /// </summary>
        public static async Task<List<Project>> GetProjects()
        {
            return await BaseUrl.AppendPathSegment("projects")
                .GetJsonAsync<List<Project>>();
        }

        /// <summary>
        /// Gets the complete list of projects that are registered for the specified Launch year
        /// </summary>
        public static async Task<LaunchProjects> GetLaunchProjects(int year)
        {
            return await BaseUrl.AppendPathSegments("projects", "launch", year.ToString())
                .GetJsonAsync<LaunchProjects>();
        }

        /// <summary>
        /// Gets the project with the specified ID
        /// </summary>
        public static async Task<Project> GetProject(int projectId)
        {
            return await BaseUrl.AppendPathSegments("projects", "id", projectId.ToString())
                .GetJsonAsync<Project>();
        }

        /// <summary>
        /// Gets the list of collaborators for the specified project
        /// </summary>
        public static async Task<List<Collaborator>> GetProjectCollaborators(int projectId)
        {
            return await BaseUrl.AppendPathSegments("projects", "collaborators")
                .SetQueryParam(nameof(projectId), projectId.ToString())
                .GetJsonAsync<List<Collaborator>>();
        }

        /// <summary>
        /// Registers a project with the given details
        /// </summary>
#if NET_STANDARD_14
        public static async Task<HttpResponseMessage> PostProject(Project info)
#else
        public static async Task<IFlurlResponse> PostProject(Project info)
#endif
        {
            return await BaseUrl.AppendPathSegments("projects")
                .WithOAuthBearerToken(Token)
                .SendUrlEncodedAsync(HttpMethod.Post, info);
        }

        /// <summary>
        /// Updates the project with the given details
        /// </summary>
#if NET_STANDARD_14
        public static async Task<HttpResponseMessage> PutProject(string appName, Project info)
#else
        public static async Task<IFlurlResponse> PutProject(string appName, Project info)
#endif
        {
            return await BaseUrl.AppendPathSegments("projects")
                .SetQueryParam(nameof(appName), appName)
                .WithOAuthBearerToken(Token)
                .SendJsonAsync(HttpMethod.Put, info);
        }

        /// <summary>
        /// Deletes the project that matches the app name the closest
        /// </summary>
#if NET_STANDARD_14
        public static async Task<HttpResponseMessage> DeleteProject(DeleteProjectRequest info)
#else
        public static async Task<IFlurlResponse> DeleteProject(DeleteProjectRequest info)
#endif
        {
            return await BaseUrl.AppendPathSegments("projects")
                .WithOAuthBearerToken(Token)
                .SendUrlEncodedAsync(HttpMethod.Delete, info);
        }
        #endregion

        #region /user/
        /// <summary>
        /// Gets the user's registered projects (requires authentication)
        /// </summary>
        /// <param name="userId">The Discord ID of the user</param>
        public static async Task<List<Project>> GetUserProjects(string userId)
        {
            return await BaseUrl.AppendPathSegments("user", userId, "projects")
                .WithOAuthBearerToken(Token)
                .GetJsonAsync<List<Project>>();
        }

        /// <summary>
        /// Gets the user's profile information
        /// </summary>
        /// <param name="userId">The Discord ID of the user</param>
        public static async Task<Collaborator> GetUser(string userId)
        {
            return await BaseUrl.AppendPathSegments("user", userId)
                .GetJsonAsync<Collaborator>();
        }

        /// <summary>
        /// Sets the user's profile information
        /// </summary>
#if NET_STANDARD_14
        public static async Task<HttpResponseMessage> SetUser(Dictionary<string, string> newInfo)
#else
        public static async Task<IFlurlResponse> SetUser(Dictionary<string, string> newInfo)
#endif
        {
            return await BaseUrl.AppendPathSegments("user")
                .WithOAuthBearerToken(Token)
                .SendUrlEncodedAsync(HttpMethod.Put, newInfo);
        }

        /// <summary>
        /// Creates a user with the specified profile information
        /// </summary>
#if NET_STANDARD_14
        public static async Task<HttpResponseMessage> PostUser(Dictionary<string, string> info)
#else
        public static async Task<IFlurlResponse> PostUser(Dictionary<string, string> info)
#endif
        {
            return await BaseUrl.AppendPathSegments("user")
                .WithOAuthBearerToken(Token)
                .SendUrlEncodedAsync(HttpMethod.Post, info);
        }
        #endregion

        #region /bot/
        /// <summary>
        /// Gets the user's profile information from Discord
        /// </summary>
        public static async Task<Discord.Models.User> GetDiscordUser(string userId)
        {
            // TODO: Deserialize to a class rather than returning a dynamic
            return await BaseUrl.AppendPathSegments("bot", "user", userId)
                .GetJsonAsync<Discord.Models.User>();
        }

        /// <summary>
        /// Gets the user's roles in the UWP Community Discord server
        /// </summary>
        public static async Task<List<Discord.Models.Role>> GetDiscordUserRoles(string userId)
        {
            // TODO: Deserialize to a class rather than returning a dynamic
            return await BaseUrl.AppendPathSegments("bot", "user", userId, "roles")
                .GetJsonAsync<List<Discord.Models.Role>>();
        }
        #endregion

        #region /signin/
#if NET_STANDARD_14
        public static async Task<HttpResponseMessage> Redirect(string code)
#else
        public static async Task<IFlurlResponse> Redirect(string code)
#endif
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
            string responseString;
#if NET_STANDARD_14
            responseString = await httpResponse.Content.ReadAsStringAsync();
#else
            responseString = await httpResponse.GetStringAsync();
#endif

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
    }
}
