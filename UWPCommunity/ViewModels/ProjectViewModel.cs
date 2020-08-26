using UWPCommLib.Api.UWPComm.Models;

namespace UWPCommunity.ViewModels
{
    public class ProjectViewModel
    {
        public Project project;

        public ProjectViewModel(Project project)
        {
            this.project = project;
        }

        public bool IsOwner => project.IsOwner(Common.DiscordUser.DiscordId);
        public bool IsDeveloper => project.IsDeveloper(Common.DiscordUser.DiscordId);
        public bool IsTranslator => project.IsTranslator(Common.DiscordUser.DiscordId);
        public bool IsBetaTester => project.IsBetaTester(Common.DiscordUser.DiscordId);
    }
}
