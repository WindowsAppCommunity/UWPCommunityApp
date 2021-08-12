using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Net.Http;
using UwpCommunityBackend.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LaunchView : Page
    {
        public LaunchView()
        {
            InitializeComponent();
            Loaded += LaunchView_Loaded;
        }

        private async void LaunchView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Common.IsInternetAvailable()) return;
            RefreshProjects();

            try
            {
                await ViewModel.InitializeAsync();
            }
            catch (HttpRequestException ex)
            {
                NavigationManager.NavigateToReconnect(ex);
            }
        }

        private async void RefreshProjects()
        {
            try
            {
                await ViewModel.RefreshProjects();
                LoadingIndicator.Visibility = Visibility.Collapsed;
            }
            catch (HttpRequestException e)
            {
                NavigationManager.Navigate(typeof(Subviews.NoInternetPage), e);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.PersistantProject = e.Parameter as Project;

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Launch: Navigated to",
                new System.Collections.Generic.Dictionary<string, string> {
                    { "From", e.SourcePageType.Name },
                    { "Parameters", e.Parameter?.ToString() }
                }
            );
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateToDashboard();
        }

        private void ParticipantsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Project item = ParticipantsGridView.SelectedItem as Project;
            //ParticipantsGridView.PrepareConnectedAnimation("projectView", item, "HeroImageStartCtl");
            NavigationManager.NavigateToViewProject(item);
        }

        private void ParticipantsGridView_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.PersistantProject != null)
            {
                ParticipantsGridView.ScrollIntoView(ViewModel.PersistantProject);
                //ConnectedAnimation animation =
                //    ConnectedAnimationService.GetForCurrentView().GetAnimation("projectView");
                //if (animation != null)
                //{
                //    await ParticipantsGridView.TryStartConnectedAnimationAsync(
                //        animation, PersistantProject, "HeroImageStartCtl");
                //}
            }
        }

        private void RefreshContainer_RefreshRequested(Microsoft.UI.Xaml.Controls.RefreshContainer sender, Microsoft.UI.Xaml.Controls.RefreshRequestedEventArgs args)
        {
            RefreshProjects();
        }
    }
}
