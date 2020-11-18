using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace UWPCommunity
{
    public static class Common
    {
        public static Discord.Models.User DiscordUser { get; private set; }
        public static async Task<UwpCommunityBackend.Models.Collaborator> GetCurrentUser()
        {
            return await UwpCommunityBackend.Api.GetUser(DiscordUser.DiscordId);
        }

        public static bool IsLoggedIn = false;
        public delegate void OnLoginStateChangedHandler(bool isLoggedIn);
        public static event OnLoginStateChangedHandler OnLoginStateChanged;

        public static async Task<bool> SignIn(string discordToken, string refreshToken)
        {
            try
            {
                Discord.Api.Token = discordToken;
                UwpCommunityBackend.Api.Token = discordToken;
                DiscordUser = await Discord.Api.GetCurrentUser();

                var vault = new Windows.Security.Credentials.PasswordVault();
                vault.Add(new Windows.Security.Credentials.PasswordCredential(
                    resourceName, DiscordUser.DiscordId, refreshToken));

                try
                {
                    _ = await GetCurrentUser();
                }
                catch (ApiException ex)
                {
                    var error = await ex.GetContentAsAsync<UwpCommunityBackend.Models.Error>();
                    if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // The user does not exist yet, so create an account for them
                        await UwpCommunityBackend.Api.PostUser(new Dictionary<string, string>() {
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
                resourceName, DiscordUser.DiscordId, null));

            IsLoggedIn = false;
            UwpCommunityBackend.Api.Token = "";
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

        public static Windows.UI.Xaml.Visibility BoolToVisibility(bool value)
		{
            return value ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
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
}
