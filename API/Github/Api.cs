using Flurl;
using Flurl.Http;
using GitHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Github
{
    public static class Api
    {
        public const string BASE_URL = "https://api.github.com";
        private const string USER_AGENT = "UWP Community App";

        /// <summary>
        /// Gets the list of all collaborators for the sepcified repo
        /// </summary>
        public static async Task<List<Contributor>> GetContributors(string owner, string repo)
        {
            return await BASE_URL.AppendPathSegments("repos", owner, repo, "contributors")
                .WithHeader("User-Agent", USER_AGENT)
                .GetJsonAsync<List<Contributor>>();
        }

        /// <summary>
        /// Gets specified user
        /// </summary>
        public static async Task<GitHubUser> GetUser(string user)
        {
            return await BASE_URL.AppendPathSegments("users", user)
                .WithHeader("User-Agent", USER_AGENT)
                .GetJsonAsync<GitHubUser>();
        }
    }
}
