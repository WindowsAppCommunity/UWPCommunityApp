using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;
using UWPCommLib.Api.Yoshi.Models;

namespace UWPCommLib.Api.Yoshi
{
    [Headers("Origin: uwpcommunity://")]
    public interface IYoshiApi
    {
        #region App Messages
        [Get("/AppMessages")]
        Task<Dictionary<string, List<AppMessage>>> GetAppMessages();

        [Get("/AppMessages/{appName}")]
        Task<List<AppMessage>> GetAppMessages(string appName);

        [Get("/AppMessages/{appName}?messageId={messageId}")]
        Task<List<AppMessage>> GetAppMessages(string appName, int messageId);
        #endregion
    }
}
