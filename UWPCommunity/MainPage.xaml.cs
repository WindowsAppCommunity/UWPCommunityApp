using Fluent.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using NavigationViewSelectionChangedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPCommunity
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            UserManager.OnLoginStateChanged += Common_OnLoginStateChanged;
            MainFrame.Navigated += MainFrame_Navigated;
            Loaded += MainPage_Loaded;
            NavigationManager.PageFrame = MainFrame;

            SizeChanged += MainPage_SizeChanged;

            foreach (PageInfo page in Pages)
            {
                MainNav.MenuItems.Add(page.NavViewItem);
            }
            MainNav.SelectedItem = MainNav.MenuItems[0];

            (MainNav.MenuItems[3] as NavigationViewItem).Visibility =
                SettingsManager.GetShowLlamaBingo() ? Visibility.Visible : Visibility.Collapsed;
            SettingsManager.ShowLlamaBingoChanged += SettingsManager_ShowLlamaBingoChanged;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ShowLatestAppMessage();
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 640)
            {
                VisualStateManager.GoToState(this, "Normal", false);
                Window.Current.SetTitleBar(null);
            }
            else
            {
                VisualStateManager.GoToState(this, "Compact", false);
                Window.Current.SetTitleBar(TitlebarBorder);
            }
        }

        private void SettingsManager_ShowLlamaBingoChanged(bool newValue)
        {
            (MainNav.MenuItems[3] as NavigationViewItem).Visibility =
                newValue ? Visibility.Visible : Visibility.Collapsed;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!Common.IsInternetAvailable())
            {
                var appFrame = Window.Current.Content as Frame;
                appFrame.Navigate(typeof(Views.Subviews.NoInternetPage));
            }
            else
            {
                await UserManager.SignInFromVault(false);
                UpdateSignInUI();

                if (e.Parameter is Tuple<Type, object> launchInfo && launchInfo.Item1 != null)
                    NavigationManager.Navigate(launchInfo.Item1, launchInfo.Item2);
            }
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            MainNav.IsBackEnabled = MainFrame.CanGoBack;
            try
            {
                // Update the NavView when the frame navigates on its own.
                // This is in a try-catch block so that I don't have to do a dozen
                // null checks.
                var page = Pages.Find((info) => info.PageType == e.SourcePageType);
                if (page == null)
                {
                    MainNav.SelectedItem = null;
                    return;
                }
                MainNav.SelectedItem = MainNav.MenuItems.ToList().Find((obj) => (obj as NavigationViewItem).Tag == page);
            }
            catch
            {
                MainNav.SelectedItem = null;
            }
        }

        private void Common_OnLoginStateChanged(bool isLoggedIn)
        {
            UpdateSignInUI(isLoggedIn);
        }
        private void UpdateSignInUI(bool? isLoggedIn = null)
        {
            if (!isLoggedIn.HasValue)
                isLoggedIn = UserManager.IsLoggedIn;

            if (isLoggedIn.Value)
            {
                SignInButton.Visibility = Visibility.Collapsed;
                UserButton.Visibility = Visibility.Visible;
                UserProfilePicture.ProfilePicture =
                    new Windows.UI.Xaml.Media.Imaging.BitmapImage(UserManager.DiscordUser.AvatarUri);
                AutomationProperties.SetName(UserButton, UserManager.DiscordUser.Username);
                ToolTipService.SetToolTip(UserButton, UserManager.DiscordUser.Username);
                UserProfileName.Text = UserManager.DiscordUser.Username;
            }
            else
            {
                SignInButton.Visibility = Visibility.Visible;
                UserButton.Visibility = Visibility.Collapsed;
            }

            // Update navigation items that require authentication
            foreach (object menuItem in MainNav.MenuItems)
            {
                if (!(menuItem is NavigationViewItem navItem))
                    continue;

                if (navItem.Tag is PageInfo info && info.RequiresAuth)
                {
                    navItem.Visibility = isLoggedIn.Value ? Visibility.Visible : Visibility.Collapsed;
                    if (!isLoggedIn.Value && navItem.IsSelected)
                    {
                        // Navigate away from the page
                        NavigationManager.NavigateToHome();
                    }
                }
            }
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                NavigationManager.NavigateToSettings();
                return;
            }

            if (!(args.SelectedItem is NavigationViewItem navItem))
            {
                NavigationManager.NavigateToHome();
                return;
            }

            PageInfo pageInfo = navItem.Tag as PageInfo;
            if (pageInfo == null)
            {
                NavigationManager.NavigateToHome();
                return;
            }            

            if (pageInfo != null && pageInfo.PageType.BaseType == typeof(Page))
                MainFrame.Navigate(pageInfo.PageType);
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.RequestSignIn(typeof(Views.DashboardView));
        }
        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            UserManager.SignOut();
        }

        public static List<PageInfo> Pages = new List<PageInfo>
        {
            new PageInfo()
            {
                PageType = typeof(Views.HomeView),
                Icon = new FluentIconElement(FluentSymbol.Home24),
                Title = "Home",
                Subhead = "The front page of the UWP Community",
                Path = "home"
            },

            new PageInfo()
            {
                PageType = typeof(Views.ProjectsView),
                Icon = new FluentIconElement(FluentSymbol.Library24),
                Title = "Projects",
                Subhead = "Explore registered projects",
                Path = "projects"
            },

            new PageInfo()
            {
                PageType = typeof(Views.LaunchView),
                Icon = new FluentIconElement(FluentSymbol.Rocket24),
                Title = "Launch",
                Subhead = "Explore Launch participants",
                Path = "launch"
            },

            new PageInfo()
            {
                PageType = typeof(Views.Subviews.LlamaBingo),
                Icon = new PathIcon {
                    Data = (Geometry)Windows.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(Geometry), "F1 M 20.527344 18.193359 L 21.230469 18.896484 L 21.230469 25.791016 L 17.021484 25.791016 L 17.021484 23.037109 L 15.839844 25.791016 L 11.435547 25.791016 L 13.505859 20.605469 L 13.505859 20.117188 C 13.356119 20.143229 13.208008 20.159506 13.061523 20.166016 C 12.915039 20.172525 12.766926 20.175781 12.617188 20.175781 L 10 20.175781 L 10 22.158203 L 9.296875 22.861328 L 9.296875 25.791016 L 5.087891 25.791016 L 5.087891 19.912109 C 4.775391 19.788412 4.482422 19.62565 4.208984 19.423828 C 3.935546 19.222006 3.694661 18.990885 3.486328 18.730469 C 3.277995 18.470053 3.103841 18.18685 2.963867 17.880859 C 2.823893 17.574871 2.727864 17.252605 2.675781 16.914063 L 1.777344 11.054688 L 1.230469 11.054688 C 0.891927 11.054688 0.572917 10.989584 0.273438 10.859375 C -0.026042 10.729167 -0.286458 10.553386 -0.507813 10.332031 C -0.729167 10.110678 -0.904948 9.850261 -1.035156 9.550781 C -1.165365 9.251303 -1.230469 8.932293 -1.230469 8.59375 L -1.230469 8.017578 L 0.878906 5.908203 L 0.878906 4.980469 C 0.878906 4.752605 0.922852 4.539389 1.010742 4.34082 C 1.098633 4.142254 1.217448 3.9681 1.367188 3.818359 C 1.516927 3.668621 1.691081 3.549805 1.889648 3.461914 C 2.088216 3.374023 2.301432 3.330078 2.529297 3.330078 C 2.770182 3.330078 3.004557 3.382162 3.232422 3.486328 C 3.343099 3.434246 3.457031 3.395184 3.574219 3.369141 C 3.691406 3.3431 3.808594 3.330078 3.925781 3.330078 C 4.153646 3.330078 4.365234 3.372396 4.560547 3.457031 C 4.755859 3.541668 4.934896 3.662109 5.097656 3.818359 L 5.244141 3.964844 C 5.413411 4.134115 5.572916 4.295248 5.722656 4.448242 C 5.872396 4.601238 6.002604 4.763998 6.113281 4.936523 C 6.223958 5.109051 6.313477 5.296225 6.381836 5.498047 C 6.450195 5.69987 6.48763 5.93099 6.494141 6.191406 C 6.585286 6.321615 6.671549 6.453451 6.75293 6.586914 C 6.83431 6.720379 6.904297 6.86198 6.962891 7.011719 L 8.603516 11.044922 L 13.457031 11.044922 C 13.697916 11.044922 13.94043 11.041667 14.18457 11.035156 C 14.428711 11.028646 14.674479 11.025391 14.921875 11.025391 C 15.221354 11.025391 15.524088 11.035156 15.830078 11.054688 C 16.103516 10.826823 16.401367 10.65267 16.723633 10.532227 C 17.045898 10.411784 17.379557 10.351563 17.724609 10.351563 C 18.108723 10.351563 18.47168 10.424805 18.813477 10.571289 C 19.155273 10.717773 19.453125 10.917969 19.707031 11.171875 C 19.960938 11.425781 20.161133 11.723633 20.307617 12.06543 C 20.4541 12.407227 20.527342 12.773438 20.527344 13.164063 C 20.527342 13.580729 20.436195 13.98112 20.253906 14.365234 C 20.345051 14.619141 20.41341 14.881186 20.458984 15.151367 C 20.504555 15.42155 20.527342 15.693359 20.527344 15.966797 Z M 19.824219 24.384766 L 19.824219 19.472656 L 19.121094 18.769531 L 19.121094 15.966797 C 19.121094 15.335287 18.961588 14.74935 18.642578 14.208984 C 18.792316 14.078776 18.909504 13.920898 18.994141 13.735352 C 19.078775 13.549805 19.121094 13.35612 19.121094 13.154297 C 19.121094 12.965495 19.085285 12.786459 19.013672 12.617188 C 18.942057 12.447917 18.842773 12.299805 18.71582 12.172852 C 18.588867 12.045898 18.440754 11.944987 18.271484 11.870117 C 18.102213 11.795248 17.919922 11.757813 17.724609 11.757813 C 17.451172 11.757813 17.20052 11.829428 16.972656 11.972656 C 16.744791 12.115886 16.572266 12.311198 16.455078 12.558594 C 16.168619 12.486979 15.888672 12.451172 15.615234 12.451172 L 8.603516 12.451172 C 8.199869 12.451172 7.851563 12.294922 7.558594 11.982422 C 7.480469 11.897787 7.394205 11.75944 7.299805 11.567383 C 7.205403 11.375326 7.114258 11.170248 7.026367 10.952148 C 6.938477 10.73405 6.855469 10.522461 6.777344 10.317383 C 6.699219 10.112305 6.640625 9.957683 6.601563 9.853516 L 5.664063 7.548828 C 5.598958 7.392579 5.517578 7.244467 5.419922 7.104492 C 5.322266 6.964519 5.211588 6.839193 5.087891 6.728516 C 5.087891 6.442058 5.076497 6.220703 5.053711 6.064453 C 5.030924 5.908203 4.985352 5.773112 4.916992 5.65918 C 4.848633 5.545248 4.749349 5.428061 4.619141 5.307617 C 4.488932 5.187176 4.316406 5.019531 4.101563 4.804688 C 4.049479 4.752605 3.994141 4.726563 3.935547 4.726563 C 3.863932 4.726563 3.80371 4.750977 3.754883 4.799805 C 3.706054 4.848633 3.68164 4.908855 3.681641 4.980469 L 3.681641 6.142578 C 3.662109 5.99935 3.618164 5.867514 3.549805 5.74707 C 3.481445 5.626628 3.401692 5.512695 3.310547 5.405273 C 3.219401 5.297852 3.120117 5.195313 3.012695 5.097656 C 2.905273 5 2.802734 4.902344 2.705078 4.804688 C 2.652994 4.752605 2.5944 4.726563 2.529297 4.726563 C 2.457682 4.726563 2.39746 4.750977 2.348633 4.799805 C 2.299804 4.848633 2.27539 4.908855 2.275391 4.980469 L 2.275391 6.542969 C 2.145182 6.634115 2.027995 6.731771 1.923828 6.835938 L 0.175781 8.59375 L 1.572266 8.59375 C 1.572266 8.782553 1.503906 8.94694 1.367188 9.086914 C 1.230469 9.226889 1.067708 9.296875 0.878906 9.296875 L 0.439453 9.296875 C 0.608724 9.485678 0.826823 9.599609 1.09375 9.638672 C 1.165364 9.651693 1.258138 9.659831 1.37207 9.663086 C 1.486002 9.666342 1.611328 9.667969 1.748047 9.667969 C 1.975911 9.667969 2.205403 9.664714 2.436523 9.658203 C 2.667643 9.651693 2.848307 9.648438 2.978516 9.648438 L 4.0625 16.689453 C 4.108073 16.988934 4.204102 17.265625 4.350586 17.519531 C 4.49707 17.773438 4.677734 17.993164 4.892578 18.178711 C 5.107422 18.364258 5.351563 18.509115 5.625 18.613281 C 5.898438 18.717447 6.18815 18.769531 6.494141 18.769531 L 6.494141 24.384766 L 7.890625 24.384766 L 7.890625 22.285156 L 8.59375 21.582031 L 8.59375 18.769531 L 12.763672 18.769531 C 12.815754 18.769531 12.871094 18.769531 12.929688 18.769531 C 12.988281 18.769531 13.043619 18.766275 13.095703 18.759766 C 13.492838 18.720703 13.876953 18.601889 14.248047 18.40332 C 14.619141 18.204752 14.928385 17.949219 15.175781 17.636719 C 15.364582 17.8125 15.550129 17.993164 15.732422 18.178711 C 15.914713 18.364258 16.097004 18.551432 16.279297 18.740234 L 18.417969 20.878906 L 18.417969 24.384766 Z M 14.912109 24.384766 L 16.71875 20.166016 L 15.166016 18.613281 C 15.081379 18.684896 14.996744 18.75 14.912109 18.808594 L 14.912109 20.878906 L 13.505859 24.384766 Z M 3.330078 8.242188 C 3.232421 8.242188 3.149414 8.208008 3.081055 8.139648 C 3.012695 8.071289 2.978515 7.988281 2.978516 7.890625 L 2.978516 7.1875 C 2.978515 7.096355 3.014322 7.014975 3.085938 6.943359 C 3.157552 6.871745 3.238932 6.835938 3.330078 6.835938 C 3.421224 6.835938 3.502604 6.871745 3.574219 6.943359 C 3.645833 7.014975 3.68164 7.096355 3.681641 7.1875 L 3.681641 7.890625 C 3.68164 7.988281 3.64746 8.071289 3.579102 8.139648 C 3.510742 8.208008 3.427734 8.242188 3.330078 8.242188 Z "), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center
                },
                Title = "Llamingo",
                Subhead = "Play XAML Llama's 'Llamingo'",
                Path = "llamabingo"
            },

            new PageInfo()
            {
                PageType = typeof(Views.DashboardView),
                Icon = new FluentIconElement(FluentSymbol.Board24),
                Title = "Dashboard",
                Subhead = "Manage and register your apps",
                Path = "dashboard",
                Visibility = Visibility.Collapsed,
                RequiresAuth = true
            },
        };

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Views.Dialogs.EditProfileDialog();
            dialog.ShowAsync();
        }

        private void PreferencesButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateToSettings(SettingsPages.AppMessages);
        }

        private async void ShowLatestAppMessage()
        {
            int level = SettingsManager.AppMessageSettings.GetImportanceLevel();
            if (level <= 0)
                return;

            try
            {
                // Load most recent app message
                var message = (await YoshiServer.Api.GetAppMessages("UWPCommunity", 0))[0];
                if (message.Id != SettingsManager.AppMessageSettings.GetLastAppMessageId()
                    && message.Importance <= level)
                {
                    var date = new DateTime(1970, 1, 1).AddSeconds(message.Timestamp).ToLocalTime();

                    MessageBox.Title = message.Title;
                    MessageContentBox.Text = message.Message;
                    MessageTimestampRun.Text = date.ToShortDateString() + " " + date.ToShortTimeString();
                    MessageAuthorRun.Text = message.Author;
                    MessageBox.IsOpen = true;
                    SettingsManager.AppMessageSettings.SetLastAppMessageId(message.Id);
                }
            }
            catch (Flurl.Http.FlurlHttpException)
            {
                // Ignore error
            }
        }
    }
}
