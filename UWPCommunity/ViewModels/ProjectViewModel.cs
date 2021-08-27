using Microsoft.Toolkit.Mvvm.ComponentModel;
using UwpCommunityBackend.Models;

namespace UWPCommunity.ViewModels
{
    public class ProjectViewModel : ObservableObject
    {
        public ProjectViewModel(Project project)
        {
            Project = project;

            CardSizeChanged(SettingsManager.GetProjectCardSize());
            SettingsManager.ProjectCardSizeChanged += CardSizeChanged;
        }

        private void CardSizeChanged(Windows.Foundation.Point cardSize)
        {
            CardWidth = cardSize.X;
            CardHeight = cardSize.Y;
        }

        private Project _Project;
        public Project Project
        {
            get => _Project;
            set => SetProperty(ref _Project, value);
        }

        public bool IsOwner {
            get
			{
                if (UserManager.DiscordUser == null || !Project.IsCollaboratorsAvailable)
                    return false;
                return Project.IsOwner(UserManager.DiscordUser.DiscordId);
            }
        }
        public bool IsDeveloper
        {
            get
            {
                if (UserManager.DiscordUser == null || !Project.IsCollaboratorsAvailable)
                    return false;
                return Project.IsDeveloper(UserManager.DiscordUser.DiscordId);
            }
        }
        public bool IsTranslator
        {
            get
            {
                if (UserManager.DiscordUser == null || !Project.IsCollaboratorsAvailable)
                    return false;
                return Project.IsTranslator(UserManager.DiscordUser.DiscordId);
            }
        }
        public bool IsBetaTester
        {
            get
            {
                if (UserManager.DiscordUser == null || !Project.IsCollaboratorsAvailable)
                    return false;
                return Project.IsBetaTester(UserManager.DiscordUser.DiscordId);
            }
        }

        private double _CardWidth;
        public double CardWidth
        {
            get => _CardWidth;
            set => SetProperty(ref _CardWidth, value);
        }

        private double _CardHeight;
        public double CardHeight
        {
            get => _CardHeight;
            set => SetProperty(ref _CardHeight, value);
        }
    }
}
