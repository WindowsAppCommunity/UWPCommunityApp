using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
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

            Common.OnLoginStateChanged += Common_OnLoginStateChanged;
            MainFrame.Navigated += MainFrame_Navigated;
            NavigationManager.PageFrame = MainFrame;

            foreach (PageInfo page in Pages)
            {
                MainNav.MenuItems.Add(new Microsoft.UI.Xaml.Controls.NavigationViewItem()
                {
                    Content = page.Title,
                    Icon = page.Icon,
                    Visibility = page.Visibility,
                });
            }
            MainNav.SelectedItem = MainNav.MenuItems[0];

            (MainNav.MenuItems[3] as Microsoft.UI.Xaml.Controls.NavigationViewItem).Visibility =
                SettingsManager.GetShowLlamaBingo() ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
            SettingsManager.ShowLlamaBingoChanged += SettingsManager_ShowLlamaBingoChanged;
        }

        private void SettingsManager_ShowLlamaBingoChanged(bool newValue)
        {
            (MainNav.MenuItems[3] as Microsoft.UI.Xaml.Controls.NavigationViewItem).Visibility =
                newValue ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await Common.TrySignIn(false);
            UpdateSignInUI();

            Tuple<Type, object> launchInfo = e.Parameter as Tuple<Type, object>;
            if (launchInfo != null && launchInfo.Item1 != null)
                NavigationManager.Navigate(launchInfo.Item1, launchInfo.Item2);

            base.OnNavigatedTo(e);
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
                MainNav.SelectedItem = MainNav.MenuItems.ToList().Find((obj) => (obj as Microsoft.UI.Xaml.Controls.NavigationViewItem).Content.ToString() == page.Title);
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
                isLoggedIn = Common.IsLoggedIn;

            if (isLoggedIn.Value)
            {
                UnloadObject(SignInButton);
                FindName("UserButton");
                UserProfilePicture.ProfilePicture =
                    new Windows.UI.Xaml.Media.Imaging.BitmapImage(Common.DiscordUser.AvatarUri);
                (MainNav.MenuItems[4] as Microsoft.UI.Xaml.Controls.NavigationViewItem).Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                FindName("SignInButton");
                UnloadObject(UserButton);
                (MainNav.MenuItems[4] as Microsoft.UI.Xaml.Controls.NavigationViewItem).Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                NavigationManager.NavigateToSettings();
                return;
            }

            Microsoft.UI.Xaml.Controls.NavigationViewItem navItem = args.SelectedItem as Microsoft.UI.Xaml.Controls.NavigationViewItem;
            if (navItem == null)
            {
                NavigationManager.NavigateToHome();
                return;
            }

            PageInfo pageInfo = Pages.Find((info) => info.Title == navItem.Content.ToString());
            if (pageInfo == null)
            {
                NavigationManager.NavigateToHome();
                return;
            }            

            if (pageInfo != null && pageInfo.PageType.BaseType == typeof(Page))
                MainFrame.Navigate(pageInfo.PageType);
        }

        private void SignInButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            NavigationManager.RequestSignIn(typeof(Views.DashboardView));
        }
        private void SignOutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Common.SignOut();
        }

        public static List<PageInfo> Pages = new List<PageInfo>
        {
            new PageInfo()
            {
                PageType = typeof(Views.HomeView),
                Icon = new SymbolIcon(Symbol.Home),
                Title = "Home",
                Subhead = "The front page of the UWP Community",
                Path = "home"
            },

            new PageInfo()
            {
                PageType = typeof(Views.ProjectsView),
                Icon = new SymbolIcon(Symbol.Library),
                Title = "Projects",
                Subhead = "Explore registered projects",
                Path = "projects"
            },

            new PageInfo()
            {
                PageType = typeof(Views.LaunchView),
                Icon = new PathIcon {
                    Data = (Geometry)Windows.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(Geometry), "F1 M 20 0 L 20 0.625 C 20 1.809896 19.894205 2.916668 19.682617 3.945313 C 19.471027 4.973959 19.150391 5.947266 18.720703 6.865234 C 18.291016 7.783203 17.75065 8.657227 17.099609 9.487305 C 16.448566 10.317383 15.686849 11.126303 14.814453 11.914063 L 12.949219 17.5 L 10 17.5 L 10 15.283203 C 9.101563 15.810547 8.193359 16.318359 7.275391 16.806641 L 3.193359 12.724609 C 3.68164 11.806641 4.189453 10.898438 4.716797 10 L 2.5 10 L 2.5 7.050781 L 8.085938 5.195313 C 8.873697 4.316406 9.682617 3.551434 10.512695 2.900391 C 11.342773 2.24935 12.216797 1.708984 13.134766 1.279297 C 14.052734 0.849609 15.026041 0.528973 16.054688 0.317383 C 17.083332 0.105795 18.190104 0 19.375 0 Z M 5.449219 8.75 C 5.638021 8.450521 5.82845 8.154297 6.020508 7.861328 C 6.212565 7.568359 6.41276 7.278646 6.621094 6.992188 L 3.75 7.949219 L 3.75 8.75 Z M 7.509766 15.253906 C 7.841797 15.065104 8.173828 14.87793 8.505859 14.692383 C 8.837891 14.506836 9.169922 14.319662 9.501953 14.130859 L 5.869141 10.498047 C 5.680338 10.830078 5.493164 11.162109 5.307617 11.494141 C 5.12207 11.826172 4.934896 12.161459 4.746094 12.5 Z M 13.007813 13.378906 C 12.721354 13.58724 12.431641 13.787436 12.138672 13.979492 C 11.845703 14.17155 11.549479 14.361979 11.25 14.550781 L 11.25 16.25 L 12.050781 16.25 Z M 14.794922 10.185547 C 15.439452 9.541016 16.000977 8.885092 16.479492 8.217773 C 16.958008 7.550456 17.360025 6.853842 17.685547 6.12793 C 18.011066 5.402019 18.26009 4.637045 18.432617 3.833008 C 18.605143 3.028973 18.707682 2.171225 18.740234 1.259766 C 17.828775 1.292318 16.971027 1.394857 16.166992 1.567383 C 15.362955 1.73991 14.597981 1.988934 13.87207 2.314453 C 13.146158 2.639975 12.447916 3.041992 11.777344 3.520508 C 11.106771 3.999023 10.445963 4.557293 9.794922 5.195313 C 9.150391 5.826824 8.561197 6.492514 8.027344 7.192383 C 7.493489 7.892253 6.992188 8.626303 6.523438 9.394531 L 10.605469 13.476563 C 11.373697 13.007813 12.106119 12.504883 12.802734 11.967773 C 13.499349 11.430664 14.163411 10.836589 14.794922 10.185547 Z M 12.5 10 C 12.154947 10 11.831055 9.934896 11.52832 9.804688 C 11.225586 9.674479 10.960286 9.495443 10.732422 9.267578 C 10.504557 9.039714 10.325521 8.774414 10.195313 8.47168 C 10.065104 8.168945 10 7.845053 10 7.5 C 10 7.154948 10.065104 6.831055 10.195313 6.52832 C 10.325521 6.225586 10.504557 5.960287 10.732422 5.732422 C 10.960286 5.504558 11.225586 5.325521 11.52832 5.195313 C 11.831055 5.065105 12.154947 5.000001 12.5 5 C 12.845052 5.000001 13.168945 5.065105 13.47168 5.195313 C 13.774414 5.325521 14.039713 5.504558 14.267578 5.732422 C 14.495442 5.960287 14.674479 6.225586 14.804688 6.52832 C 14.934895 6.831055 14.999999 7.154948 15 7.5 C 14.999999 7.845053 14.934895 8.168945 14.804688 8.47168 C 14.674479 8.774414 14.495442 9.039714 14.267578 9.267578 C 14.039713 9.495443 13.774414 9.674479 13.47168 9.804688 C 13.168945 9.934896 12.845052 10 12.5 10 Z M 12.5 6.25 C 12.324219 6.25 12.161458 6.282553 12.011719 6.347656 C 11.861979 6.412761 11.730143 6.502279 11.616211 6.616211 C 11.502278 6.730145 11.41276 6.86198 11.347656 7.011719 C 11.282552 7.161459 11.25 7.324219 11.25 7.5 C 11.25 7.675781 11.282552 7.838542 11.347656 7.988281 C 11.41276 8.138021 11.502278 8.269857 11.616211 8.383789 C 11.730143 8.497722 11.861979 8.58724 12.011719 8.652344 C 12.161458 8.717448 12.324219 8.75 12.5 8.75 C 12.675781 8.75 12.838541 8.717448 12.988281 8.652344 C 13.13802 8.58724 13.269855 8.497722 13.383789 8.383789 C 13.497721 8.269857 13.587239 8.138021 13.652344 7.988281 C 13.717447 7.838542 13.75 7.675781 13.75 7.5 C 13.75 7.324219 13.717447 7.161459 13.652344 7.011719 C 13.587239 6.86198 13.497721 6.730145 13.383789 6.616211 C 13.269855 6.502279 13.13802 6.412761 12.988281 6.347656 C 12.838541 6.282553 12.675781 6.25 12.5 6.25 Z M 2.5 15 C 2.845052 15 3.168945 15.065104 3.47168 15.195313 C 3.774414 15.325521 4.039713 15.504558 4.267578 15.732422 C 4.495442 15.960287 4.674479 16.225586 4.804688 16.52832 C 4.934896 16.831055 5 17.154949 5 17.5 C 5 17.845053 4.934896 18.168945 4.804688 18.47168 C 4.674479 18.774414 4.495442 19.039713 4.267578 19.267578 C 4.039713 19.495443 3.774414 19.674479 3.47168 19.804688 C 3.168945 19.934896 2.845052 20 2.5 20 L 0 20 L 0 17.5 C 0 17.154949 0.065104 16.831055 0.195313 16.52832 C 0.325521 16.225586 0.504557 15.960287 0.732422 15.732422 C 0.960286 15.504558 1.225586 15.325521 1.52832 15.195313 C 1.831055 15.065104 2.154948 15 2.5 15 Z M 2.5 18.75 C 2.675781 18.75 2.838542 18.717447 2.988281 18.652344 C 3.138021 18.58724 3.269857 18.497721 3.383789 18.383789 C 3.497721 18.269857 3.58724 18.138021 3.652344 17.988281 C 3.717448 17.838541 3.75 17.675781 3.75 17.5 C 3.75 17.324219 3.717448 17.161459 3.652344 17.011719 C 3.58724 16.861979 3.497721 16.730143 3.383789 16.616211 C 3.269857 16.502279 3.138021 16.41276 2.988281 16.347656 C 2.838542 16.282553 2.675781 16.25 2.5 16.25 C 2.324219 16.25 2.161458 16.282553 2.011719 16.347656 C 1.861979 16.41276 1.730143 16.502279 1.616211 16.616211 C 1.502279 16.730143 1.41276 16.861979 1.347656 17.011719 C 1.282552 17.161459 1.25 17.324219 1.25 17.5 L 1.25 18.75 Z "), HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center, VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center
                },
                Title = "Launch",
                Subhead = "Explore Launch participants",
                Path = "launch"
            },

            new PageInfo()
            {
                PageType = typeof(Views.Subviews.LlamaBingo),
                Icon = new PathIcon {
                    Data = (Geometry)Windows.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(Geometry), "F1 M 20.527344 18.193359 L 21.230469 18.896484 L 21.230469 25.791016 L 17.021484 25.791016 L 17.021484 23.037109 L 15.839844 25.791016 L 11.435547 25.791016 L 13.505859 20.605469 L 13.505859 20.117188 C 13.356119 20.143229 13.208008 20.159506 13.061523 20.166016 C 12.915039 20.172525 12.766926 20.175781 12.617188 20.175781 L 10 20.175781 L 10 22.158203 L 9.296875 22.861328 L 9.296875 25.791016 L 5.087891 25.791016 L 5.087891 19.912109 C 4.775391 19.788412 4.482422 19.62565 4.208984 19.423828 C 3.935546 19.222006 3.694661 18.990885 3.486328 18.730469 C 3.277995 18.470053 3.103841 18.18685 2.963867 17.880859 C 2.823893 17.574871 2.727864 17.252605 2.675781 16.914063 L 1.777344 11.054688 L 1.230469 11.054688 C 0.891927 11.054688 0.572917 10.989584 0.273438 10.859375 C -0.026042 10.729167 -0.286458 10.553386 -0.507813 10.332031 C -0.729167 10.110678 -0.904948 9.850261 -1.035156 9.550781 C -1.165365 9.251303 -1.230469 8.932293 -1.230469 8.59375 L -1.230469 8.017578 L 0.878906 5.908203 L 0.878906 4.980469 C 0.878906 4.752605 0.922852 4.539389 1.010742 4.34082 C 1.098633 4.142254 1.217448 3.9681 1.367188 3.818359 C 1.516927 3.668621 1.691081 3.549805 1.889648 3.461914 C 2.088216 3.374023 2.301432 3.330078 2.529297 3.330078 C 2.770182 3.330078 3.004557 3.382162 3.232422 3.486328 C 3.343099 3.434246 3.457031 3.395184 3.574219 3.369141 C 3.691406 3.3431 3.808594 3.330078 3.925781 3.330078 C 4.153646 3.330078 4.365234 3.372396 4.560547 3.457031 C 4.755859 3.541668 4.934896 3.662109 5.097656 3.818359 L 5.244141 3.964844 C 5.413411 4.134115 5.572916 4.295248 5.722656 4.448242 C 5.872396 4.601238 6.002604 4.763998 6.113281 4.936523 C 6.223958 5.109051 6.313477 5.296225 6.381836 5.498047 C 6.450195 5.69987 6.48763 5.93099 6.494141 6.191406 C 6.585286 6.321615 6.671549 6.453451 6.75293 6.586914 C 6.83431 6.720379 6.904297 6.86198 6.962891 7.011719 L 8.603516 11.044922 L 13.457031 11.044922 C 13.697916 11.044922 13.94043 11.041667 14.18457 11.035156 C 14.428711 11.028646 14.674479 11.025391 14.921875 11.025391 C 15.221354 11.025391 15.524088 11.035156 15.830078 11.054688 C 16.103516 10.826823 16.401367 10.65267 16.723633 10.532227 C 17.045898 10.411784 17.379557 10.351563 17.724609 10.351563 C 18.108723 10.351563 18.47168 10.424805 18.813477 10.571289 C 19.155273 10.717773 19.453125 10.917969 19.707031 11.171875 C 19.960938 11.425781 20.161133 11.723633 20.307617 12.06543 C 20.4541 12.407227 20.527342 12.773438 20.527344 13.164063 C 20.527342 13.580729 20.436195 13.98112 20.253906 14.365234 C 20.345051 14.619141 20.41341 14.881186 20.458984 15.151367 C 20.504555 15.42155 20.527342 15.693359 20.527344 15.966797 Z M 19.824219 24.384766 L 19.824219 19.472656 L 19.121094 18.769531 L 19.121094 15.966797 C 19.121094 15.335287 18.961588 14.74935 18.642578 14.208984 C 18.792316 14.078776 18.909504 13.920898 18.994141 13.735352 C 19.078775 13.549805 19.121094 13.35612 19.121094 13.154297 C 19.121094 12.965495 19.085285 12.786459 19.013672 12.617188 C 18.942057 12.447917 18.842773 12.299805 18.71582 12.172852 C 18.588867 12.045898 18.440754 11.944987 18.271484 11.870117 C 18.102213 11.795248 17.919922 11.757813 17.724609 11.757813 C 17.451172 11.757813 17.20052 11.829428 16.972656 11.972656 C 16.744791 12.115886 16.572266 12.311198 16.455078 12.558594 C 16.168619 12.486979 15.888672 12.451172 15.615234 12.451172 L 8.603516 12.451172 C 8.199869 12.451172 7.851563 12.294922 7.558594 11.982422 C 7.480469 11.897787 7.394205 11.75944 7.299805 11.567383 C 7.205403 11.375326 7.114258 11.170248 7.026367 10.952148 C 6.938477 10.73405 6.855469 10.522461 6.777344 10.317383 C 6.699219 10.112305 6.640625 9.957683 6.601563 9.853516 L 5.664063 7.548828 C 5.598958 7.392579 5.517578 7.244467 5.419922 7.104492 C 5.322266 6.964519 5.211588 6.839193 5.087891 6.728516 C 5.087891 6.442058 5.076497 6.220703 5.053711 6.064453 C 5.030924 5.908203 4.985352 5.773112 4.916992 5.65918 C 4.848633 5.545248 4.749349 5.428061 4.619141 5.307617 C 4.488932 5.187176 4.316406 5.019531 4.101563 4.804688 C 4.049479 4.752605 3.994141 4.726563 3.935547 4.726563 C 3.863932 4.726563 3.80371 4.750977 3.754883 4.799805 C 3.706054 4.848633 3.68164 4.908855 3.681641 4.980469 L 3.681641 6.142578 C 3.662109 5.99935 3.618164 5.867514 3.549805 5.74707 C 3.481445 5.626628 3.401692 5.512695 3.310547 5.405273 C 3.219401 5.297852 3.120117 5.195313 3.012695 5.097656 C 2.905273 5 2.802734 4.902344 2.705078 4.804688 C 2.652994 4.752605 2.5944 4.726563 2.529297 4.726563 C 2.457682 4.726563 2.39746 4.750977 2.348633 4.799805 C 2.299804 4.848633 2.27539 4.908855 2.275391 4.980469 L 2.275391 6.542969 C 2.145182 6.634115 2.027995 6.731771 1.923828 6.835938 L 0.175781 8.59375 L 1.572266 8.59375 C 1.572266 8.782553 1.503906 8.94694 1.367188 9.086914 C 1.230469 9.226889 1.067708 9.296875 0.878906 9.296875 L 0.439453 9.296875 C 0.608724 9.485678 0.826823 9.599609 1.09375 9.638672 C 1.165364 9.651693 1.258138 9.659831 1.37207 9.663086 C 1.486002 9.666342 1.611328 9.667969 1.748047 9.667969 C 1.975911 9.667969 2.205403 9.664714 2.436523 9.658203 C 2.667643 9.651693 2.848307 9.648438 2.978516 9.648438 L 4.0625 16.689453 C 4.108073 16.988934 4.204102 17.265625 4.350586 17.519531 C 4.49707 17.773438 4.677734 17.993164 4.892578 18.178711 C 5.107422 18.364258 5.351563 18.509115 5.625 18.613281 C 5.898438 18.717447 6.18815 18.769531 6.494141 18.769531 L 6.494141 24.384766 L 7.890625 24.384766 L 7.890625 22.285156 L 8.59375 21.582031 L 8.59375 18.769531 L 12.763672 18.769531 C 12.815754 18.769531 12.871094 18.769531 12.929688 18.769531 C 12.988281 18.769531 13.043619 18.766275 13.095703 18.759766 C 13.492838 18.720703 13.876953 18.601889 14.248047 18.40332 C 14.619141 18.204752 14.928385 17.949219 15.175781 17.636719 C 15.364582 17.8125 15.550129 17.993164 15.732422 18.178711 C 15.914713 18.364258 16.097004 18.551432 16.279297 18.740234 L 18.417969 20.878906 L 18.417969 24.384766 Z M 14.912109 24.384766 L 16.71875 20.166016 L 15.166016 18.613281 C 15.081379 18.684896 14.996744 18.75 14.912109 18.808594 L 14.912109 20.878906 L 13.505859 24.384766 Z M 3.330078 8.242188 C 3.232421 8.242188 3.149414 8.208008 3.081055 8.139648 C 3.012695 8.071289 2.978515 7.988281 2.978516 7.890625 L 2.978516 7.1875 C 2.978515 7.096355 3.014322 7.014975 3.085938 6.943359 C 3.157552 6.871745 3.238932 6.835938 3.330078 6.835938 C 3.421224 6.835938 3.502604 6.871745 3.574219 6.943359 C 3.645833 7.014975 3.68164 7.096355 3.681641 7.1875 L 3.681641 7.890625 C 3.68164 7.988281 3.64746 8.071289 3.579102 8.139648 C 3.510742 8.208008 3.427734 8.242188 3.330078 8.242188 Z "), HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center, VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center
                },
                Title = "Llamingo",
                Subhead = "Play XAML Llama's 'Llamingo'",
                Path = "llamabingo"
            },

            new PageInfo()
            {
                PageType = typeof(Views.DashboardView),
                Icon = new FontIcon() {
                    Glyph = "\uF246",
                    FontFamily = Common.SegoeMDL2Assets
                },
                Title = "Dashboard",
                Subhead = "Manage and register your apps",
                Path = "dashboard",
                Visibility = Windows.UI.Xaml.Visibility.Collapsed
            },
        };

        private void EditProfileButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var dialog = new Views.Dialogs.EditProfileDialog();
            dialog.ShowAsync();
        }
    }
}
