using System.Collections.ObjectModel;
using UWPCommLib.Api.UWPComm.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProjectsView : Page
    {
        public ObservableCollection<Project> Projects { get; set; } = new ObservableCollection<Project>();

        public ProjectsView()
        {
            InitializeComponent();

            var cardSize = SettingsManager.GetProjectCardSize();
            ProjectsGridView.DesiredWidth = cardSize.X;
            ProjectsGridView.ItemHeight = cardSize.Y;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var projs = await Common.UwpCommApi.GetProjects();
            foreach (var project in projs)
            {
                Projects.Add(project);
            }
            base.OnNavigatedTo(e);
        }

        private async void ExternalLinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var project = (sender as Button)?.DataContext as Project;
            await NavigationManager.OpenInBrowser(project.ExternalLink);
        }

        private async void GitHubLinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var project = (sender as Button)?.DataContext as Project;
            await NavigationManager.OpenInBrowser(project.GitHubLink);
        }

        private async void DownloadLinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var project = (sender as Button)?.DataContext as Project;
            await NavigationManager.OpenInBrowser(project.DownloadLink);
        }

        private void ProjectsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Project item = ProjectsGridView.SelectedItem as Project;
            ProjectsGridView.PrepareConnectedAnimation("projectView", item, "HeroImageStartCtl");
            NavigationManager.NavigateToViewProject(item);
        }
    }
}
