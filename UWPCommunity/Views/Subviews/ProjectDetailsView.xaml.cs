using System;
using System.Linq;
using System.Threading.Tasks;
using UwpCommunityBackend;
using UwpCommunityBackend.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views.Subviews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProjectDetailsView : Page
    {
        public ProjectDetailsView()
        {
            this.InitializeComponent();
        }

        public Project Project { get; set; }
        private Type PreviousPage;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PreviousPage = e.SourcePageType;

            if (e.Parameter is Project project)
            {
                Project = project;
                UpdateCollaboratorsBlock();
            }

            base.OnNavigatedTo(e);
        }

        private async Task UpdateCollaboratorsBlock()
        {
            try
            {
                Project.Collaborators = await Api.GetProjectCollaborators(Project.Id);

                var collaboratorNames = Project.Collaborators
                    .Where(c => c.IsOwner)
                    .Select(c => c.Name);

                await Dispatcher.RunAsync(default, () =>
                {
                    CollaboratorsBlock.Text += string.Join(", ", collaboratorNames);
                });
            }
            catch { }
        }

        private async void ExternalLinkButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigationManager.OpenInBrowser(Project.ExternalLink);
        }

        private async void GitHubLinkButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigationManager.OpenInBrowser(Project.GitHubLink);
        }

        private async void DownloadLinkButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigationManager.OpenInBrowser(Project.DownloadLink);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.PageFrame.GoBack();
        }
    }
}
