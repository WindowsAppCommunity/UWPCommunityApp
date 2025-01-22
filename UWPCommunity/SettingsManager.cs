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
            if (!localSettings.Values.ContainsKey("ExtendIntoTitleBar") || overrideCurr)
                SetExtendIntoTitleBar(true);
        }

        public static async void ResetApp()
        {
            // TODO: This currently doesn't work, because the app is still running.
            await ApplicationData.Current.ClearAsync();
        }

        public static ElementTheme GetAppTheme()
        {
            return ThemeFromName(GetAppThemeName());
        }
        public static string GetAppThemeName()
        {
            if (localSettings.Values.TryGetValue("AppTheme", out object value))
            {
                return value.ToString();
            }
            else
            {
                var defaultTheme = "Default";
                SetAppTheme(defaultTheme);
                return defaultTheme;
            }
        }
        public static void SetAppTheme(ElementTheme theme)
        {
            localSettings.Values["AppTheme"] = theme.ToString("g");
            ApplyAppTheme(theme);
            AppThemeChanged?.Invoke(theme);
            SettingsChanged?.Invoke("AppTheme", theme);
        }
        public static void SetAppTheme(string themeString)
        {
            SetAppTheme(ThemeFromName(themeString));
        }
        private static ElementTheme ThemeFromName(string themeName)
        {
            return (ElementTheme)Enum.Parse(typeof(ElementTheme), themeName, true);
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
            if (localSettings.Values.TryGetValue("UseDebugApi", out object value))
            {
                // Older versions stored this a string
                if (value is bool boolVal)
                    return boolVal;
                else
                    return bool.Parse(value.ToString());
            }
            else
            {
                SetUseDebugApi(true);
                return true;
            }
        }
        public static void SetUseDebugApi(bool value)
        {
            localSettings.Values["UseDebugApi"] = value;
            ApplyUseDebugApi(value);
            UseDebugApiChanged?.Invoke(value);
            SettingsChanged?.Invoke("UseDebugApi", value);
        }
        public static void ApplyUseDebugApi(bool value)
        {
            UwpCommunityBackend.Api.BaseUrl =
                value ?
                UwpCommunityBackend.Api.LOCAL_BASE_URL :
                UwpCommunityBackend.Api.WEB_BASE_URL;
        }
        public delegate void UseDebugApiChangedHandler(bool value);
        public static event UseDebugApiChangedHandler UseDebugApiChanged;

        public static Point GetProjectCardSize()
        {
            if (localSettings.Values.TryGetValue("ProjectCardSize", out object value))
            {
                return (Point)value;
            }
            else
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
            SettingsChanged?.Invoke("ProjectCardSize", value);
        }
        public delegate void ProjectCardSizeChangedHandler(Point value);
        public static event ProjectCardSizeChangedHandler ProjectCardSizeChanged;

        public static bool GetShowLlamaBingo()
        {
            if (localSettings.Values.TryGetValue("ShowLlamaBingo", out object value))
            {
                return (bool)value;
            }
            else
            {
                SetShowLlamaBingo(true);
                return true;
            }
        }
        public static void SetShowLlamaBingo(bool value)
        {
            localSettings.Values["ShowLlamaBingo"] = value;
            ShowLlamaBingoChanged?.Invoke(value);
            SettingsChanged?.Invoke("ShowLlamaBingo", value);
        }
        public delegate void ShowLlamaBingoChangedHandler(bool value);
        public static event ShowLlamaBingoChangedHandler ShowLlamaBingoChanged;

        public static string GetSavedLlamaBingo()
        {
            if (localSettings.Values.TryGetValue("SavedLlamaBingo", out object value))
            {
                return value.ToString();
            }
            else
            {
                SetSavedLlamaBingo(null);
                return null;
            }
        }
        public static void SetSavedLlamaBingo(string boardData)
        {
            localSettings.Values["SavedLlamaBingo"] = boardData;
            SavedLlamaBingoChanged?.Invoke(boardData);
            SettingsChanged?.Invoke("SavedLlamaBingo", boardData);
        }
        public delegate void SavedLlamaBingoChangedHandler(string boardData);
        public static event SavedLlamaBingoChangedHandler SavedLlamaBingoChanged;

        public static bool GetExtendIntoTitleBar()
        {
            if (localSettings.Values.TryGetValue("ExtendIntoTitleBar", out object value))
            {
                return (bool)value;
            }
            else
            {
                SetExtendIntoTitleBar(true);
                return true;
            }
        }
        public static void SetExtendIntoTitleBar(bool value)
        {
            localSettings.Values["ExtendIntoTitleBar"] = value;
            ExtendIntoTitleBarChanged?.Invoke(value);
            SettingsChanged?.Invoke("ExtendIntoTitleBar", value);
        }
        public delegate void ExtendIntoTitleBarChangedHandler(bool value);
        public static event ExtendIntoTitleBarChangedHandler ExtendIntoTitleBarChanged;

        public delegate void SettingsChangedHandler(string name, object value);
        public static event SettingsChangedHandler SettingsChanged;
    }
}
