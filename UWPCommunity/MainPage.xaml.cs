using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            Common.OnLoginStateChanged += Common_OnLoginStateChanged;
            MainFrame.Navigated += MainFrame_Navigated;
            NavigationManager.PageFrame = MainFrame;
            base.OnNavigatedTo(e);
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // TODO: Update the NavView when the frame navigates on its own
            MainNav.IsBackEnabled = MainFrame.CanGoBack;
            var page = Pages.ToList().Find((info) => info.PageType == e.SourcePageType);
            MainNav.SelectedItem = page == null ? null : page;
        }

        private void Common_OnLoginStateChanged(bool isLoggedIn)
        {
            if (isLoggedIn)
            {
                UnloadObject(SignInButton);
                FindName("UserButton");
                UserProfilePicture.ProfilePicture =
                    new Windows.UI.Xaml.Media.Imaging.BitmapImage(Common.DiscordUser.AvatarUri);
                ((ObservableCollection<PageInfo>)MainNav.MenuItemsSource)[3].Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                FindName("SignInButton");
                UnloadObject(UserButton);
                ((ObservableCollection<PageInfo>)MainNav.MenuItemsSource)[3].Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem == null) NavigationManager.NavigateToHome();

            if (args.IsSettingsSelected)
            {
                if (args.IsSettingsSelected)
                    MainFrame.Navigate(typeof(Views.HomeView));
                return;
            }

            PageInfo pageInfo = (PageInfo)args.SelectedItem;
            if (pageInfo != null && pageInfo.PageType.BaseType == typeof(Page))
                MainFrame.Navigate(pageInfo.PageType, null, args.RecommendedNavigationTransitionInfo);
        }

        private void SignInButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            NavigationManager.RequestSignIn(typeof(Views.DashboardView));
        }
        private void SignOutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Common.SignOut();
        }

        public ObservableCollection<PageInfo> Pages = new ObservableCollection<PageInfo>
        {
            new PageInfo()
            {
                PageType = typeof(Views.HomeView),
                Icon = new SymbolIcon(Symbol.Home),
                Title = "Home",
            },

            new PageInfo()
            {
                PageType = typeof(Views.ProjectsView),
                Icon = new SymbolIcon(Symbol.Library),
                Title = "Projects",
            },

            new PageInfo()
            {
                PageType = typeof(Views.LaunchView),
                Icon = new FontIcon() {
                    Glyph = "\uF3B3",
                    FontFamily = Common.FabricMDL2Assets
                },
                Title = "Launch",
            },

            new PageInfo()
            {
                PageType = typeof(Views.DashboardView),
                Icon = new FontIcon() {
                    Glyph = "\uF246",
                    FontFamily = Common.FabricMDL2Assets
                },
                Title = "Dashboard",
                Visibility = Windows.UI.Xaml.Visibility.Collapsed
            },
        };        
    }
}
