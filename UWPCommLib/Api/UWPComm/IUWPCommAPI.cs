using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using UWPCommLib.Api.UWPComm.Models;

namespace UWPCommLib.Api.UWPComm
{
    public interface IUWPCommAPI
    {
        [Get("/projects")]
        Task<List<Project>> GetProjects();

        [Get("/user/{userId}/projects")]
        Task<List<Project>> GetUserProjects(string userId);

        [Get("/user/{userId}")]
        Task<List<Project>> GetUser(string userId);
    }
}
