using System;
using System.Collections.Generic;
using UwpCommunityBackend.Models;
using UWPCommunity.Views.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomeView : Page
    {
        public HomeView()
        {
            this.InitializeComponent();
            Loaded += HomeView_Loaded;
        }

        private async void HomeView_Loaded(object sender, RoutedEventArgs e)
        {
            // Get the card information from the website frontend
            var card = (await UwpCommunityBackend.Api.GetCard("home")).Main;
            CardSubtitle.Text = card.Subtitle;
            CardDetails.Text = String.Join("\r\n", card.Details);
            try
            {
                SettingsManager.ApplyLiveTile(SettingsManager.GetShowLiveTile());
            }
            catch (Flurl.Http.FlurlHttpException)
            {
                var appFrame = Window.Current.Content as Frame;
                appFrame.Navigate(typeof(Subviews.NoInternetPage));
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Home: Navigated to",
                new Dictionary<string, string> {
                    { "From", e.SourcePageType.Name },
                    { "Parameters", e.Parameter?.ToString() }
                }
            );
        }

        private async void DiscordButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigationManager.OpenDiscordInvite(Common.DISCORD_INVITE);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Home: Discord button clicked");
        }

        private void Launch2020Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate(typeof(LaunchView));
        }

        private async void GitHubButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Home: GitHub button clicked");
            await NavigationManager.OpenInBrowser("https://github.com/UWPCommunity/");
        }

        private async void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Home: Launch button clicked");
            await NavigationManager.OpenInBrowser("https://medium.com/@Arlodottxt/uwp-community-launch-2020-1772efb1e382");
        }
    }
}
