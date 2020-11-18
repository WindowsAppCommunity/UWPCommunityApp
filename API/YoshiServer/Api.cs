using Flurl;
using Flurl.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using YoshiServer.Models;

namespace YoshiServer
{
    public static class Api
    {
        public const string BASE_URL = "https://yoshiask.herokuapp.com/api";

        #region App Messages
        public static async Task<Dictionary<string, List<AppMessage>>> GetAppMessages()
        {
            return await BASE_URL.AppendPathSegment("AppMessages")
                .GetJsonAsync<Dictionary<string, List<AppMessage>>>();
        }

        public static async Task<List<AppMessage>> GetAppMessages(string appName)
        {
            return await BASE_URL.AppendPathSegments("AppMessages", appName)
                .GetJsonAsync<List<AppMessage>>();
        }

        public static async Task<List<AppMessage>> GetAppMessages(string appName, int messageId)
        {
            return await BASE_URL.AppendPathSegments("AppMessages", appName)
                .SetQueryParam(nameof(messageId), messageId)
                .GetJsonAsync<List<AppMessage>>();
        }
        #endregion
    }
}
