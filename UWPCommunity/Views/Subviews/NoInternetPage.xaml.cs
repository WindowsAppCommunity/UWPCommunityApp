using System;
using System.Net.NetworkInformation;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views.Subviews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NoInternetPage : Page
    {
        public NoInternetPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Keep checking for an internet connection and
            // return to previous page if reconnected
            while (true)
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    await Launcher.LaunchUriAsync(new Uri("uwpcommunity://"));
                    return;
                }
            }
        }
    }
}
