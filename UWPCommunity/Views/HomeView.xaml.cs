using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using UWPCommunity.Views.Dialogs;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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

            TileBindingContentAdaptive text = new TileBindingContentAdaptive
            {
                Children =
                {
                    new AdaptiveText()
                    {
                        Text = "Launch 2020",
                        HintWrap = true,
                    },
                    new AdaptiveText()
                    {
                        Text = "The UWP Community is nearing its annual Launch event",
                        HintStyle = AdaptiveTextStyle.CaptionSubtle,
                        HintWrap = true
                    }
                }
            };
            var tileContent = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = new TileBinding()
                    {
                        Branding = TileBranding.Logo,
                        Content = text
                    },
                    TileWide = new TileBinding()
                    {
                        Branding = TileBranding.NameAndLogo,
                        Content = text
                    },
                    TileLarge = new TileBinding()
                    {
                        Branding = TileBranding.NameAndLogo,
                        Content = text
                    }
                }
            };
            var notification = new TileNotification(tileContent.GetXml());
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);

            ShowLatestAppMessage();
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
            await NavigationManager.OpenDiscordInvite("eBHZSKG");
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Home: Discord button clicked");
        }

        private void Launch2020Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate(typeof(Views.LaunchView));
        }

        private async void GitHubButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Home: GitHub button clicked");
            await NavigationManager.OpenInBrowser("https://github.com/UWPCommunity/");
        }

        private async void Launch2019Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Home: Launch 2019 button clicked");
            await NavigationManager.OpenInBrowser("https://medium.com/@Arlodottxt/launch-2019-7efd37cc0877");
        }

        private async void ShowLatestAppMessage()
        {
            int level = SettingsManager.AppMessageSettings.GetImportanceLevel();
            if (level == 0)
                return;

            // Load most recent app message
            var message = (await Common.YoshiApi.GetAppMessages("UWPCommunity", 0))[0];
            if (message.Id == SettingsManager.AppMessageSettings.GetLastAppMessageId()
                || message.Importance >= level)
            {
                await new AppMessageDialog(message).ShowAsync();
                SettingsManager.AppMessageSettings.SetLastAppMessageId(message.Id);
            }
        }
    }
}
