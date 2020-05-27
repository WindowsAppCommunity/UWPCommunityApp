using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using UWPCommLib.Api.Discord.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginView : Page
    {
        private Type DestinationPage;

        public LoginView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationManager.RemovePreviousFromBackStack();
            LoginWrapper.NavigationCompleted += LoginWrapper_NavigationCompleted;
            LoginWrapper.Navigate(new Uri("https://discordapp.com/api/oauth2/authorize?client_id=611491369470525463&redirect_uri=http%3A%2F%2Fuwpcommunity-site-backend.herokuapp.com%2Fsignin%2Fredirect&response_type=code&scope=identify%20guilds"));
            
            Type page = e.Parameter as Type;
            DestinationPage = page == null ? typeof(HomeView) : page;

            base.OnNavigatedTo(e);

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Sign in: auth flow started",
                new System.Collections.Generic.Dictionary<string, string> {
                    { "From", e.SourcePageType.Name },
                    { "Parameters", e.Parameter?.ToString() }
                }
            );
        }

        private async void LoginWrapper_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            const string redBaseUrl = "https://uwpcommunity.com/signin?authResponse=";
            string redirect = args.Uri.AbsoluteUri;
            if (redirect.StartsWith(redBaseUrl))
            {
                sender.Visibility = Visibility.Collapsed;
                LoadingIndicator.Visibility = Visibility.Visible;

                var authResponseBase64 = redirect.Replace(redBaseUrl, "");
                byte[] data = Convert.FromBase64String(authResponseBase64);
                var authResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(System.Text.Encoding.ASCII.GetString(data));

                await Common.SignIn(authResponse.Token, authResponse.RefreshToken);

                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Sign in: success");

                NavigationManager.Navigate(DestinationPage);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            LoginWrapper.NavigationCompleted -= LoginWrapper_NavigationCompleted;
        }
    }
}
