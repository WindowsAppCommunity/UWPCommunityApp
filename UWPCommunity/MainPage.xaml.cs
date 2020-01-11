using Refit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPCommLib.Api.UWPComm.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            MainNav.SelectedItem = MainNav.MenuItems[0];
            base.OnNavigatedTo(e);
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            Type navTo = typeof(MainPage);
            if (args.IsSettingsSelected)
            {
                navTo = typeof(MainPage);
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
                    //navTo = typeof(Views.LaunchView);
                    break;
            }

            MainFrame.Navigate(navTo, null, args.RecommendedNavigationTransitionInfo);
        }
    }
}
