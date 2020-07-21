using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UWPCommLib.Api.Discord;
using UWPCommLib.Api.UWPComm;
using Windows.UI.Xaml.Media;
using UWPCommLib.Api.Yoshi;
using System.Linq;

namespace UWPCommunity
{
    public static class Common
    {
        public static string UwpCommApiHostUrl = "https://uwpcommunity-site-backend.herokuapp.com";
        public static IUwpCommApi UwpCommApi = RestService.For<IUwpCommApi>(
            UwpCommApiHostUrl
        );
        public static IYoshiApi YoshiApi = RestService.For<IYoshiApi>(
            "https://yoshiask.herokuapp.com/api"
        );
        public static IDiscordApi DiscordApi = RestService.For<IDiscordApi>(
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
                    UwpCommApiHostUrl,
                    new RefitSettings()
                    {
                        AuthorizationHeaderValueGetter = () => Task.FromResult(_token)
                    }
                );
                DiscordApi = RestService.For<IDiscordApi>(
                    "https://discordapp.com/api",
                    new RefitSettings()
                    {
                        AuthorizationHeaderValueGetter = () => Task.FromResult(_token)
                    }
                );
            }
        }
        public static string DiscordRefreshToken { get; set; }

        public static UWPCommLib.Api.Discord.Models.User DiscordUser { get; private set; }
        public static async Task<UWPCommLib.Api.UWPComm.Models.Collaborator> GetCurrentUser()
        {
            return await UwpCommApi.GetUser(DiscordUser.DiscordId);
        }

        public static bool IsLoggedIn = false;
        public delegate void OnLoginStateChangedHandler(bool isLoggedIn);
        public static event OnLoginStateChangedHandler OnLoginStateChanged;

        public static async Task<bool> SignIn(string discordToken, string refreshToken)
        {
            try
            {
                DiscordToken = discordToken;
                DiscordRefreshToken = refreshToken;
                DiscordUser = await DiscordApi.GetCurrentUser();

                var vault = new Windows.Security.Credentials.PasswordVault();
                vault.Add(new Windows.Security.Credentials.PasswordCredential(
                    resourceName, DiscordUser.DiscordId, DiscordToken));

                try
                {
                    _ = await GetCurrentUser();
                }
                catch (ApiException ex)
                {
                    var error = ex.GetContentAs<UWPCommLib.Api.UWPComm.Models.Error>();
                    if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // The user does not exist yet, so create an account for them
                        await UwpCommApi.PostUser(new Dictionary<string, string>() {
                            { "name", DiscordUser.Username },
                        });
                    }
                }

                IsLoggedIn = true;
                OnLoginStateChanged(IsLoggedIn);
            }
            catch
            {
                IsLoggedIn = false;
            }
            return IsLoggedIn;
        }
        public static async Task<bool> SignIn(string discordToken)
        {
            return await SignIn(discordToken, null);
        }
        public static async Task TrySignIn(bool useUi = true)
        {
            try
            {
                var loginCredential = GetCredentialFromLocker();

                // TODO: Figure out how to reauthenticate with UWP Comm API
                // with the Discord credential
                if (false)//loginCredential != null)
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
            var credentialList = vault.RetrieveAll();
            if (credentialList.Count > 0)
            {
                credential = credentialList[0];
            }

            return credential;
        }

        public static readonly FontFamily SegoeMDL2Assets = new FontFamily("Segoe MDL2 Assets");

        public static bool IsInternetAvailable()
        {
            var profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            return (profile != null) && !String.IsNullOrEmpty(profile.ProfileName);
        }
    }

    public static class StringExtensions
    {
        public static IEnumerable<string> TakeEvery(this string s, int count)
        {
            int index = 0;
            while (index < s.Length)
            {
                if (s.Length - index >= count)
                {
                    yield return s.Substring(index, count);
                }
                else
                {
                    yield return s.Substring(index, s.Length - index);
                }
                index += count;
            }
        }
    }

    public static class UriHelper
    {
        public static Dictionary<string, string> DecodeQueryParameters(this Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            if (uri.Query.Length == 0)
                return new Dictionary<string, string>();

            return uri.Query.TrimStart('?')
                            .Split(new[] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(parameter => parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                            .GroupBy(parts => parts[0],
                                     parts => parts.Length > 2 ? string.Join("=", parts, 1, parts.Length - 1) : (parts.Length > 1 ? parts[1] : ""))
                            .ToDictionary(grouping => grouping.Key,
                                          grouping => string.Join(",", grouping));
        }
    }
}
