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

            foreach (PageInfo page in Pages)
            {
                MainNav.MenuItems.Add(new NavigationViewItem()
                {
                    Content = page.Title,
                    Icon = page.Icon,
                    Visibility = page.Visibility,
                });
            }

            base.OnNavigatedTo(e);
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            MainNav.IsBackEnabled = MainFrame.CanGoBack;
            try
            {
                // Update the NavView when the frame navigates on its own
                //     This is in a try-catch block so that I don't have to do a dozen
                //     null checks.
                var page = Pages.Find((info) => info.PageType == e.SourcePageType);
                MainNav.SelectedItem = MainNav.MenuItems.ToList().Find((obj) => (obj as NavigationViewItem).Content.ToString() == page.Title);
            }
            catch {}
        }

        private void Common_OnLoginStateChanged(bool isLoggedIn)
        {
            if (isLoggedIn)
            {
                UnloadObject(SignInButton);
                FindName("UserButton");
                UserProfilePicture.ProfilePicture =
                    new Windows.UI.Xaml.Media.Imaging.BitmapImage(Common.DiscordUser.AvatarUri);
                (MainNav.MenuItems[3] as NavigationViewItem).Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                FindName("SignInButton");
                UnloadObject(UserButton);
                (MainNav.MenuItems[3] as NavigationViewItem).Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                // TODO: Navigate to settings
                NavigationManager.NavigateToSettings();
                return;
            }

            NavigationViewItem navItem = args.SelectedItem as NavigationViewItem;
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

        public List<PageInfo> Pages = new List<PageInfo>
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
