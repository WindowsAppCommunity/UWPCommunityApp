using System;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;

namespace UWPCommunity
{
    public static class SettingsManager
    {
        // Load the app's settings
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private static StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        public static void LoadDefaults(bool overrideCurr = true)
        {
            if (!localSettings.Values.ContainsKey("AppTheme") || overrideCurr)
                SetAppTheme("Default");
            if (!localSettings.Values.ContainsKey("UseDebugApi") || overrideCurr)
                SetUseDebugApi(false);
            if (!localSettings.Values.ContainsKey("ProjectCardSize") || overrideCurr)
                SetProjectCardSize(new Point(400, 300));
            if (!localSettings.Values.ContainsKey("ShowLlamaBingo") || overrideCurr)
                SetShowLlamaBingo(true);
            if (!localSettings.Values.ContainsKey("SavedLlamaBingo") || overrideCurr)
                SetSavedLlamaBingo(null);
        }

        public static async void ResetApp()
        {
            // TODO: This currently doesn't work, because the app is still running.
            await ApplicationData.Current.ClearAsync();
        }

        public static ElementTheme GetAppTheme()
        {
            return ThemeFromName(localSettings.Values["AppTheme"] as string);
        }
        public static string GetAppThemeName()
        {
            var theme = localSettings.Values["AppTheme"] as string;
            if (String.IsNullOrEmpty(theme))
            {
                var defaultTheme = "Default";
                SetAppTheme(defaultTheme);
                return defaultTheme;
            }
            else
            {
                return theme;
            }
        }
        public static void SetAppTheme(ElementTheme theme)
        {
            localSettings.Values["AppTheme"] = theme.ToString("g");
            ApplyAppTheme(theme);
            AppThemeChanged?.Invoke(theme);
        }
        public static void SetAppTheme(string themeString)
        {
            SetAppTheme(ThemeFromName(themeString));
        }
        private static ElementTheme ThemeFromName(string themeName)
        {
            switch (themeName)
            {
                case "Light":
                    return ElementTheme.Light;

                case "Dark":
                    return ElementTheme.Dark;

                default:
                    return ElementTheme.Default;
            }
        }
        public static void ApplyAppTheme(ElementTheme theme)
        {
            // Set theme for window root.
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = theme;
            }
        }
        public delegate void AppThemeChangedHandler(ElementTheme value);
        public static event AppThemeChangedHandler AppThemeChanged;

        public const string DEBUG_API_URL = "http://localhost:5000";
        public const string PROD_API_URL = "https://uwpcommunity-site-backend.herokuapp.com";
        public static bool GetUseDebugApi()
        {
            try
            {
                return Boolean.Parse(localSettings.Values["UseDebugApi"] as string);
            }
            catch
            {
                SetUseDebugApi(false);
                return false;
            }
        }
        public static void SetUseDebugApi(bool value)
        {
            localSettings.Values["UseDebugApi"] = value.ToString();
            ApplyUseDebugApi(value);
            UseDebugApiChanged?.Invoke(value);
        }
        public static void ApplyUseDebugApi(bool value)
        {
            Common.UwpCommApiHostUrl = value ? DEBUG_API_URL : PROD_API_URL ;
            Common.UwpCommApi = Refit.RestService.For<UWPCommLib.Api.UWPComm.IUwpCommApi>(
                Common.UwpCommApiHostUrl
            );
        }
        public delegate void UseDebugApiChangedHandler(bool value);
        public static event UseDebugApiChangedHandler UseDebugApiChanged;

        public static Point GetProjectCardSize()
        {
            try
            {
                return (Point)localSettings.Values["ProjectCardSize"];
            }
            catch
            {
                var defaultRect = new Point(300, 300);
                SetProjectCardSize(defaultRect);
                return defaultRect;
            }
        }
        public static void SetProjectCardSize(Point value)
        {
            localSettings.Values["ProjectCardSize"] = value;
            ProjectCardSizeChanged?.Invoke(value);
        }
        public delegate void ProjectCardSizeChangedHandler(Point value);
        public static event ProjectCardSizeChangedHandler ProjectCardSizeChanged;

        public static bool GetShowLlamaBingo()
        {
            try
            {
                return (bool)localSettings.Values["ShowLlamaBingo"];
            }
            catch
            {
                SetShowLlamaBingo(true);
                return true;
            }
        }
        public static void SetShowLlamaBingo(bool value)
        {
            localSettings.Values["ShowLlamaBingo"] = value;
            ShowLlamaBingoChanged?.Invoke(value);
        }
        public delegate void ShowLlamaBingoChangedHandler(bool value);
        public static event ShowLlamaBingoChangedHandler ShowLlamaBingoChanged;

        public static string GetSavedLlamaBingo()
        {
            try
            {
                return (string)localSettings.Values["SavedLlamaBingo"];
            }
            catch
            {
                SetSavedLlamaBingo(null);
                return null;
            }
        }
        public static void SetSavedLlamaBingo(string boardData)
        {
            localSettings.Values["SavedLlamaBingo"] = boardData;
            SavedLlamaBingoChanged?.Invoke(boardData);
        }
        public delegate void SavedLlamaBingoChangedHandler(string boardData);
        public static event SavedLlamaBingoChangedHandler SavedLlamaBingoChanged;
    }
}
