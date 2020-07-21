using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;
using Windows.UI.StartScreen;

namespace UWPCommunity
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += App_UnhandledException;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            ExtendIntoTitleBar();

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), NavigationManager.ParseProtocol(e.Arguments));
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
            
            SettingsManager.LoadDefaults(false);
            SettingsManager.ApplyAppTheme(SettingsManager.GetAppTheme());
            SettingsManager.ApplyUseDebugApi(SettingsManager.GetUseDebugApi());
            SetJumplist();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            this.UnhandledException += App_UnhandledException;
            Frame rootFrame = Window.Current.Content as Frame;
            ExtendIntoTitleBar();

            // Do not repeat app initialization when the Window already has content
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            Tuple<Type, object> destination = new Tuple<Type, object>(typeof(Views.HomeView), null);
            if (args.Kind == ActivationKind.Protocol)
            {
                ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;
                // TODO: Handle URI activation
                // The received URI is eventArgs.Uri.AbsoluteUri
                destination = NavigationManager.ParseProtocol(eventArgs.Uri);
            }
            rootFrame.Navigate(typeof(MainPage), destination);

            SettingsManager.ApplyAppTheme(SettingsManager.GetAppTheme());
            SettingsManager.ApplyUseDebugApi(SettingsManager.GetUseDebugApi());

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            Type exType = e.Exception.GetType();
            e.Handled = true;
            Frame rootFrame = new Frame();
            Window.Current.Content = rootFrame;

            this.UnhandledException -= App_UnhandledException;

            if (exType == typeof(HttpRequestException))
            {
                rootFrame.Navigate(typeof(Views.Subviews.NoInternetPage), e);
                return;
            }

            rootFrame.Navigate(typeof(Views.UnhandledExceptionPage), e);
        }

        private async System.Threading.Tasks.Task SetJumplist()
        {
            if (JumpList.IsSupported())
            {
                var jumpList = await JumpList.LoadCurrentAsync();
                jumpList.Items.Clear();

                foreach (PageInfo page in MainPage.Pages)
                {
                    var item = JumpListItem.CreateWithArguments(page.Protocol, page.Title);
                    item.Description = page.Subhead;
                    item.Logo = page.IconAsset;
                    jumpList.Items.Add(item);
                }

                await jumpList.SaveAsync();
            }
        }

        private void ExtendIntoTitleBar()
        {
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            if (SettingsManager.GetExtendIntoTitleBar())
            {
                coreTitleBar.ExtendViewIntoTitleBar = true;

                ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = (Color)Current.Resources["SystemAccentColor"];
                ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverForegroundColor = (Color)Current.Resources["SystemAccentColor"];
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverBackgroundColor = Colors.Transparent;
            }
            else
            {
                coreTitleBar.ExtendViewIntoTitleBar = false;

                ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = null;
                ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = null;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverForegroundColor = null;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverBackgroundColor = null;
            }
        }

        public static string GetVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }
    }
}
