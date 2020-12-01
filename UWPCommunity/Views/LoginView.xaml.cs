using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Discord.Models;
using System.Text.RegularExpressions;

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
            DestinationPage = page ?? typeof(HomeView);

            base.OnNavigatedTo(e);

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Sign in: auth flow started",
                new System.Collections.Generic.Dictionary<string, string> {
                    { "From", e.SourcePageType.Name },
                    { "Parameters", e.Parameter?.ToString() }
                }
            );
        }

        const string authRespUrl = "https://uwpcommunity.com/signin?authResponse=";
        const string errorRespUrl = "http://uwpcommunity-site-backend.herokuapp.com/signin/redirect?error=";
        bool isDialogOpen = false;
        bool removedQrCode = false;
        private async void LoginWrapper_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            await System.Threading.Tasks.Task.Delay(1500);

            if (!removedQrCode)
            {
                try
                {
                    // Execute some Javascript to remove the QR code and separator
                    await RemoveHTMLElement("qrLogin-1AOZMt");
                    await RemoveHTMLElement("verticalSeparator-3huAjp");
                    removedQrCode = true;
                }
                catch
                {
                    removedQrCode = false;
                }
            }

            string redirect = args.Uri.AbsoluteUri;
            if (redirect.StartsWith(authRespUrl))
            {
                sender.Visibility = Visibility.Collapsed;
                LoadingIndicator.Visibility = Visibility.Visible;

                var authResponseBase64 = redirect.Replace(authRespUrl, "");
                byte[] data = Convert.FromBase64String(authResponseBase64);
                var authResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(System.Text.Encoding.ASCII.GetString(data));

                await Common.SignIn(authResponse.Token, authResponse.RefreshToken);

                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Sign in: success");

                NavigationManager.Navigate(DestinationPage);
            }
            else if (redirect.StartsWith(errorRespUrl))
			{
                // Prevent the wrapper from navigating multiple times,
                // resulting in two ContentDialogs being requested at the same time
                //LoginWrapper.Stop();

                Match match = Regex.Match(redirect, @"(?:\?|&)error=(?<error>\S+)(?:\?|&)error_description=(?<error_description>\S+)");
			    switch (match?.Groups["error"]?.Value)
				{
					case "access_denied":
						// User cancelled sign in
						break;

					default:
                        if (isDialogOpen)
                            break;
                        ContentDialog dialog = new ContentDialog
                        {
                            Title = "Sign in failed",
                            Content = match.Groups["error_description"].Value.Replace("+", " "),
                            CloseButtonText = "Ok",
                            RequestedTheme = SettingsManager.GetAppTheme()
                        };
                        dialog.Closed += (s, e) => isDialogOpen = false;
                        isDialogOpen = true;
                        ContentDialogResult result = await dialog.ShowAsync();
                        break;
                }
                NavigationManager.NavigateToHome();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Do some cleanup, otherwise the WebView will stay loaded in the background.
            // Figured this out with Edge Dev Tools
            LoginWrapper.NavigationCompleted -= LoginWrapper_NavigationCompleted;
            LoginWrapper = null;
        }

        /// <summary>
        /// Removes (from the DOM) the first HTML element with the specified class name
        /// </summary>
        private async System.Threading.Tasks.Task RemoveHTMLElement(string className)
        {
            // 
            await LoginWrapper?.InvokeScriptAsync("eval", new string[]
            {
                $"var selectedElem = document.getElementsByClassName('{className}')[0];\nselectedElem.parentElement.removeChild(selectedElem);"
            });
        }
    }
}
