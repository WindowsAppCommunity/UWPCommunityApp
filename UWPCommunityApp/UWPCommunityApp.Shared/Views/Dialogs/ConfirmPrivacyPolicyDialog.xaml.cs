using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunityApp.Views.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
    }
}
