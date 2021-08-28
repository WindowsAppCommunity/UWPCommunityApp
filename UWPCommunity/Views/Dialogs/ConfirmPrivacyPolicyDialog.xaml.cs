using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views.Dialogs
{
    public sealed partial class ConfirmPrivacyPolicyDialog : ContentDialog
    {
        public ConfirmPrivacyPolicyDialog()
        {
            this.InitializeComponent();
            RequestedTheme = SettingsManager.GetAppTheme();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Do nothing. Just close the dialog and carry on with the login.
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Do nothing. The RequestSignIn method stops sign-in if the user
            // does not click the primary button.
        }

        private async void PrivacyPolicyLink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            await NavigationManager.OpenInBrowser("https://uwpcommunity.com/privacy-policy");
        }
    }
}
