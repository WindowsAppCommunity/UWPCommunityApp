using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using Github;
using GitHub.Models;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPCommunity.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsView : Page
    {
        bool isReady = false;

        public SettingsView()
        {
            this.InitializeComponent();

            #if !DEBUG
            SettingsPivot.Items.Remove(DebugTab);
            #endif

            SettingsManager.SettingsChanged += SettingsManager_SettingsChanged;
            SettingsManager.AppThemeChanged += SettingsManager_AppThemeChanged;
            SettingsManager.ProjectCardSizeChanged += SettingsManager_ProjectCardSizeChanged;
            SettingsManager.ShowLlamaBingoChanged += SettingsManager_ShowLlamaBingoChanged;
            SettingsManager.UseDebugApiChanged += SettingsManager_UseDebugApiChanged;
            SettingsManager.UseBlurEffectsChanged += SettingsManager_UseBlurEffectsChanged; ;
        }

        private void SettingsManager_SettingsChanged(string name, object value)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Settings: setting changed",
                new Dictionary<string, string> {
                    { "Setting", name + ": " + value?.ToString() },
                }
            );
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ThemeBox.SelectedValue = SettingsManager.GetAppThemeName();
            UseDebugApiBox.IsChecked = SettingsManager.GetUseDebugApi();
            var cardSize = SettingsManager.GetProjectCardSize();
            ProjectCardWidth.Value = cardSize.X;
            ProjectCardHeight.Value = cardSize.Y;
            ShowLlamaBingoBox.IsChecked = SettingsManager.GetShowLlamaBingo();
            ExtendIntoTitleBarBox.IsChecked = SettingsManager.GetExtendIntoTitleBar();
            ShowLiveTileBox.IsChecked = SettingsManager.GetShowLiveTile();
            UseBlurEffectsBox.IsChecked = SettingsManager.GetUseBlurEffects();
            ShowAppMessagesBox.IsChecked = SettingsManager.AppMessageSettings.GetShowAppMessages();
            ImportanceLevelSlider.Value = SettingsManager.AppMessageSettings.GetImportanceLevel();
            AppVersionRun.Text = App.GetVersion();
            isReady = true;

            if (e.Parameter is SettingsPages)
                SettingsPivot.SelectedIndex = (int)e.Parameter;

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Settings: Navigated to",
                new Dictionary<string, string> {
                    { "From", e.SourcePageType.Name },
                    { "Parameters", e.Parameter?.ToString() }
                }
            );
        }

        private void ShowRestartRequiredDialog()
        {
            var dialog = new ContentDialog()
            {
                Title = "Restart required",
                Content = "You modified settings that require a restart to take effect",
                IsSecondaryButtonEnabled = false,
                PrimaryButtonText = "OK"
            };
            dialog.ShowAsync();
        }

        #region Settings Manager
        private void SettingsManager_UseBlurEffectsChanged(bool value)
        {
            UseBlurEffectsBox.IsChecked = value;
        }

        private void SettingsManager_UseDebugApiChanged(bool value)
        {
            UseDebugApiBox.IsChecked = value;
        }

        private void SettingsManager_ShowLlamaBingoChanged(bool value)
        {
            ShowLlamaBingoBox.IsChecked = value;
        }

        private void SettingsManager_ProjectCardSizeChanged(Point value)
        {
            ProjectCardWidth.Value = value.X;
            ProjectCardHeight.Value = value.Y;
        }

        private void SettingsManager_AppThemeChanged(ElementTheme value)
        {
            ThemeBox.SelectedValue = SettingsManager.GetAppThemeName();
        }

        private void ThemeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SettingsManager.SetAppTheme(e.AddedItems[0].ToString());
        }

        private void UseDebugApiBox_Changed(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetUseDebugApi(UseDebugApiBox.IsChecked.Value);
        }

        private void ProjectCardSize_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            SettingsManager.SetProjectCardSize(new Point(ProjectCardWidth.Value, ProjectCardHeight.Value));
        }

        private void UseBlurEffectsBox_Checked(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetUseBlurEffects(true);
        }

        private void UseBlurEffectsBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetUseBlurEffects(false);
        }

        private void ShowLlamaBingoBox_Checked(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetShowLlamaBingo(true);
        }

        private void ShowLlamaBingoBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetShowLlamaBingo(false);
        }

        private void ExtendIntoTitleBarBox_Checked(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetExtendIntoTitleBar(true);
            if (isReady)
                ShowRestartRequiredDialog();
        }

        private void ExtendIntoTitleBarBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetExtendIntoTitleBar(false);
            if (isReady)
                ShowRestartRequiredDialog();
        }

        private void ShowLiveTileBox_Checked(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetShowLiveTile(true);
        }

        private void ShowLiveTileBox_Unchecked(object sender, RoutedEventArgs e)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            SettingsManager.SetShowLiveTile(false);
        }

        private void ShowAppMessagesBox_Checked(object sender, RoutedEventArgs e)
        {
            SettingsManager.AppMessageSettings.SetShowAppMessages(true);
        }

        private void ShowAppMessagesBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SettingsManager.AppMessageSettings.SetShowAppMessages(false);
        }

        private void ImportanceLevelSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (isReady)
                SettingsManager.AppMessageSettings.SetImportanceLevel((int)e.NewValue);
        }

        #endregion

        private async void DefaultButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Please confirm",
                Content = "Are you sure you want to reset your settings? This cannot be undone.",
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "Cancel",
                RequestedTheme = SettingsManager.GetAppTheme()
            };
            ContentDialogResult result = await dialog.ShowAsync();
            switch (result)
            {
                case ContentDialogResult.Primary:
                    SettingsManager.LoadDefaults(true);
                    break;
            }
        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Please confirm",
                Content = "Are you sure you want to reset the app? This cannot be undone.",
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "Cancel",
                RequestedTheme = SettingsManager.GetAppTheme()
            };
            ContentDialogResult result = await dialog.ShowAsync();
            switch (result)
            {
                case ContentDialogResult.Primary:
                    SettingsManager.ResetApp();
                    break;
            }
        }

        private async void CrashButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Please confirm",
                Content = "This will crash the app. Are you sure you want to do this?",
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "Cancel",
                RequestedTheme = SettingsManager.GetAppTheme()
            };
            ContentDialogResult result = await dialog.ShowAsync();
            switch (result)
            {
                case ContentDialogResult.Primary:
                    throw new Exception("User artificially threw an exception");
            }
        }

        #region Contributor Panel
        ObservableCollection<GitHubUser> Contributors { get; set; } = new ObservableCollection<GitHubUser>();
        Compositor _compositor = Window.Current.Compositor;
        SpringVector3NaturalMotionAnimation _springAnimation;

        private async void ContributorView_Loaded(object sender, RoutedEventArgs e)
        {
            var contribs = await Api.GetContributors("UWPCommunity", "UWPCommunityApp");
            Contributors.Clear();
            foreach (Contributor c in contribs)
            {
                Contributors.Add(await Api.GetUser(c.Login));
            }

            // TODO: How is this supposed to be done when items are databound?

            // Animate the translation of each button relative to the scale and translation of the button above.
            //var anim = _compositor.CreateExpressionAnimation();
            //anim.Expression = "(above.Scale.Y - 1) * 50 + above.Translation.Y % (50 * index)";
            //anim.Target = "Translation.Y";

            //var panel = sender as StackPanel;
            //for (int i = 1; i < panel.Children.Count; i++)
            //{
            //    var current = panel.Children[i];
            //    var before = panel.Children[i - 1];

            //    // Animate the second button relative to the first.
            //    anim.SetExpressionReferenceParameter("above", before);
            //    anim.SetScalarParameter("index", i);
            //    current.StartAnimation(anim);
            //}
        }

        private void CreateOrUpdateSpringAnimation(float finalValue)
        {
            if (_springAnimation == null)
            {
                _springAnimation = _compositor.CreateSpringVector3Animation();
                _springAnimation.Target = "Scale";
            }

            _springAnimation.FinalValue = new Vector3(finalValue);
        }

        private void element_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            return;
            // Scale up to 1.25
            CreateOrUpdateSpringAnimation(1.25f);

            (sender as UIElement).StartAnimation(_springAnimation);
        }

        private void element_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            return;
            // Scale back down to 1.0
            CreateOrUpdateSpringAnimation(1.0f);

            (sender as UIElement).StartAnimation(_springAnimation);
        }

        private async void ContributorView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is GitHubUser user)
            {
                await NavigationManager.OpenInBrowser(user.HtmlUrl);
            }
        }
        #endregion
    }
}
