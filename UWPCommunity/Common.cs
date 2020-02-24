using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.System;
using UWPCommLib.Api.Discord;
using UWPCommLib.Api.UWPComm;
using Windows.UI.Xaml.Media;

namespace UWPCommunity
{
    public static class Common
    {
        public static IUwpCommApi UwpCommApi = RestService.For<IUwpCommApi>(
            "https://uwpcommunity-site-backend.herokuapp.com"
        );
        public static IDiscordAPI DiscordApi = RestService.For<IDiscordAPI>(
            "https://discordapp.com/api"
        );

        static string _token;
        public static string DiscordToken
        {
            get {
                return _token;
            }
            set {
                _token = value;
                UwpCommApi = RestService.For<IUwpCommApi>(
                    "https://uwpcommunity-site-backend.herokuapp.com",
                    new RefitSettings()
                    {
                        AuthorizationHeaderValueGetter = () => Task.FromResult(_token)
                    }
                );
                DiscordApi = RestService.For<IDiscordAPI>(
                    "https://discordapp.com/api",
                    new RefitSettings()
                    {
                        AuthorizationHeaderValueGetter = () => Task.FromResult(_token)
                    }
                );
            }
        }
        public static string DiscordRefreshToken { get; set; }

        public static UWPCommLib.Api.Discord.Models.User DiscordUser { get; set; }

        public static bool IsLoggedIn = false;
        public delegate void OnLoginStateChangedHandler(bool isLoggedIn);
        public static event OnLoginStateChangedHandler OnLoginStateChanged;

        public static async Task SignIn(string discordToken, string refreshToken)
        {
            IsLoggedIn = true;
            DiscordToken = discordToken;
            DiscordRefreshToken = refreshToken;
            DiscordUser = await DiscordApi.GetCurrentUser();

            var vault = new Windows.Security.Credentials.PasswordVault();
            vault.Add(new Windows.Security.Credentials.PasswordCredential(
                resourceName, DiscordUser.DiscordId, DiscordToken));

            OnLoginStateChanged(IsLoggedIn);
        }
        public static async Task SignIn(string discordToken)
        {
            await SignIn(discordToken, null);
        }
        public static async Task TrySignIn(bool useUi = true)
        {
            try
            {
                var loginCredential = GetCredentialFromLocker();

                if (loginCredential != null)
                {
                    // There is a credential stored in the locker.
                    // Populate the Password property of the credential
                    // for automatic login.
                    loginCredential.RetrievePassword();

                    await SignIn(loginCredential.Password);
                }
                else if (useUi)
                {
                    // There is no credential stored in the locker.
                    // Display UI to get user credentials.
                    NavigationManager.RequestSignIn(typeof(Views.HomeView));
                }
            }
            catch {}
        }
        public static void SignOut()
        {
            var vault = new Windows.Security.Credentials.PasswordVault();
            vault.Remove(new Windows.Security.Credentials.PasswordCredential(
                resourceName, DiscordUser.DiscordId, DiscordToken));

            IsLoggedIn = false;
            DiscordToken = "";
            DiscordRefreshToken = "";
            DiscordUser = null;            

            OnLoginStateChanged(IsLoggedIn);
        }

        private static string resourceName = "Discord";
        private static Windows.Security.Credentials.PasswordCredential GetCredentialFromLocker()
        {
            Windows.Security.Credentials.PasswordCredential credential = null;

            var vault = new Windows.Security.Credentials.PasswordVault();
            var credentialList = vault.FindAllByResource(resourceName);
            if (credentialList.Count > 0)
            {
                credential = credentialList[0];
            }

            return credential;
        }

        public static readonly FontFamily FabricMDL2Assets = new FontFamily("Assets.ttf#Fabric External MDL2 Assets");

        public static bool IsInternetAvailable()
        {
            var profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            return (profile != null) && !String.IsNullOrEmpty(profile.ProfileName);
        }
    }
}
