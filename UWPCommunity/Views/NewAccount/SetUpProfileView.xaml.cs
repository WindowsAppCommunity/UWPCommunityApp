using System;
using System.Collections.Generic;
using UwpCommunityBackend;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views.NewAccount
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetUpProfileView : Page
    {
        public SetUpProfileView()
        {
            this.InitializeComponent();
            Loaded += EditProfileDialog_Loaded;
            RequestedTheme = SettingsManager.GetAppTheme();
        }

        private async void EditProfileDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var user = await UserManager.GetCurrentUser();
            LoadingBar.Visibility = Visibility.Collapsed;

            NameBox.Text = user.Name;
            EmailBox.Text = user.Email ?? "";
        }

        private async void ContinueButton_Click(object sender, RoutedEventArgs args)
        {
            // User wants to apply the changes
            var changes = new Dictionary<string, string>() {
                { "name", NameBox.Text }
            };
            if (!String.IsNullOrWhiteSpace(EmailBox.Text))
            {
                changes.Add("email", EmailBox.Text);
            }
            await Api.SetUser(changes);

            NavigationManager.NavigateToDashboard();
        }
    }
}
