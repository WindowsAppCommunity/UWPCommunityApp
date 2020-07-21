using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using UWPCommLib.Api.UWPComm.Models;
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
        public ObservableCollection<Project> LaunchProjects { get; set; } = new ObservableCollection<Project>();
        public Project PersistantProject;

        public string CardTitle { get; set; }
        public string CardSubtitle { get; set; }
        public string CardDetails { get; set; }

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
                // Get the card information from the website frontend
                var response = await new HttpClient().GetAsync("https://raw.githubusercontent.com/UWPCommunity/uwpcommunity.github.io/master/assets/views/launch.json");
                string json = await response.Content.ReadAsStringAsync();
                var card = Newtonsoft.Json.JsonConvert.DeserializeObject<CardInfoResponse>(json).Main;
                CardTitle = card.Title;
                CardSubtitle = card.Subtitle;
                CardDetails = String.Join(" ", card.Details);
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
                var launch = await Common.UwpCommApi.GetLaunchProjects(2020);
                LaunchProjects = new ObservableCollection<Project>(launch.Projects);
                if (ParticipantsGridView.Items.Count != LaunchProjects.Count)
                {
                    Bindings.Update();
                }
                LoadingIndicator.Visibility = Visibility.Collapsed;
            }
            catch (HttpRequestException e)
            {
                NavigationManager.Navigate(typeof(Subviews.NoInternetPage), e);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PersistantProject = e.Parameter as Project;
        }

        private void Card_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Storyboard sb = ((DropShadowPanel)sender).Resources["EnterStoryboard"] as Storyboard;
            sb.Begin();
        }

        private void Card_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Storyboard sb = ((DropShadowPanel)sender).Resources["ExitStoryboard"] as Storyboard;
            sb.Begin();
        }

        private void Launch2020Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateToDashboard();
        }

        private void ParticipantsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Project item = ParticipantsGridView.SelectedItem as Project;
            ParticipantsGridView.PrepareConnectedAnimation("projectView", item, "HeroImageStartCtl");
            NavigationManager.NavigateToViewProject(item);
        }

        private async void ParticipantsGridView_Loaded(object sender, RoutedEventArgs e)
        {
            if (PersistantProject != null)
            {
                ParticipantsGridView.ScrollIntoView(PersistantProject);
                ConnectedAnimation animation =
                    ConnectedAnimationService.GetForCurrentView().GetAnimation("projectView");
                if (animation != null)
                {
                    await ParticipantsGridView.TryStartConnectedAnimationAsync(
                        animation, PersistantProject, "HeroImageStartCtl");
                }
            }
        }

        private void RefreshContainer_RefreshRequested(Microsoft.UI.Xaml.Controls.RefreshContainer sender, Microsoft.UI.Xaml.Controls.RefreshRequestedEventArgs args)
        {
            RefreshProjects();
        }
    }
}
