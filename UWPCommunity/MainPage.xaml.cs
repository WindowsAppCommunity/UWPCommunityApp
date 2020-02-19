using Refit;
using System;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Selects and navigates to the Home page
            MainNav.SelectedItem = MainNav.MenuItems[0];
            Common.OnLoggedIn += Common_OnLoggedIn;
            base.OnNavigatedTo(e);
        }

        private void Common_OnLoggedIn()
        {
            UnloadObject(SignInButton);
            FindName("UserButton");
            DashboardNavItem.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // Set the default navigation to the Home page
            Type navTo = typeof(Views.HomeView);
            if (args.IsSettingsSelected)
            {
                // TODO: Navigate to Settings page
                navTo = typeof(Views.HomeView);
            }

            switch (((NavigationViewItem)args.SelectedItem).Content)
            {
                case "Home":
                    navTo = typeof(Views.HomeView);
                    break;

                case "Projects":
                    navTo = typeof(Views.ProjectsView);
                    break;

                case "Launch":
                    // TODO: Navigate to Launch page
                    navTo = typeof(Views.LaunchView);
                    break;

                case "Dashboard":
                    navTo = typeof(Views.Dashboard);
                    break;
            }

            // Navigate the internal frame to the selected page
            MainFrame.Navigate(navTo, null, args.RecommendedNavigationTransitionInfo);
        }

        private void SignInButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MainFrame.Navigate(typeof(Views.LoginView));
        }
    }
}
