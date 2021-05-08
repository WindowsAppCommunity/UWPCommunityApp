using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace UWPCommunity
{
    public static class UserManager
    {
        public static Discord.Models.User DiscordUser { get; private set; }
        public static async Task<UwpCommunityBackend.Models.Collaborator> GetCurrentUser()
        {
            return await UwpCommunityBackend.Api.GetUser(DiscordUser.DiscordId);
        }

        public static bool IsLoggedIn = false;
        public delegate void OnLoginStateChangedHandler(bool isLoggedIn);
        public static event OnLoginStateChangedHandler OnLoginStateChanged;

        public static async Task<bool> SignIn(string discordToken, string refreshToken = null)
        {
            try
            {
                if (discordToken == null)
                {
                    // Use refresh token to get a new token
                    (await UwpCommunityBackend.Api.UseRefreshToken(refreshToken)).Deconstruct(out var newTokens, out var error);
                    if (newTokens != null)
                    {
                        discordToken = newTokens.Token;
                        refreshToken = newTokens.RefreshToken;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(error);
                    }
                }

                Discord.Api.Token = discordToken;
                Discord.Api.RefreshToken = refreshToken;
                UwpCommunityBackend.Api.Token = discordToken;
                DiscordUser = await Discord.Api.GetCurrentUser();

                Vault.Add(new PasswordCredential(resourceName, DiscordUser.DiscordId, refreshToken));

                try
                {
                    var user = await GetCurrentUser();
                }
                catch (Flurl.Http.FlurlHttpException ex)
                {
                    var error = await ex.GetResponseJsonAsync<UwpCommunityBackend.Models.Error>();
                    if (ex.Call.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
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
            catch (Exception ex)
            {
                Discord.Api.Token = null;
                Discord.Api.RefreshToken = null;
                UwpCommunityBackend.Api.Token = null;
                IsLoggedIn = false;
            }
            return IsLoggedIn;
        }
        public static async Task<bool> SignInFromVault(bool useUi = true)
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

                    await SignIn(null, loginCredential.Password);
                }
                else if (useUi)
                {
                    // There is no credential stored in the locker.
                    // Display UI to get user credentials.
                    NavigationManager.RequestSignIn(typeof(Views.HomeView));
                }

                return IsLoggedIn;
            }
            catch
            {
                return false;
            }
        }

        public static void SignOut()
        {
            var vault = new PasswordVault();
            vault.Remove(new PasswordCredential(
                resourceName, DiscordUser.DiscordId, Discord.Api.RefreshToken));

            IsLoggedIn = false;
            UwpCommunityBackend.Api.Token = "";
            DiscordUser = null;

            OnLoginStateChanged(IsLoggedIn);
        }

        private static PasswordVault _vault;
        private static PasswordVault Vault
        {
            get
            {
                if (_vault == null)
                    _vault = new PasswordVault();
                return _vault;
            }
        }

        private static string resourceName = "Discord";
        private static PasswordCredential GetCredentialFromLocker()
        {
            PasswordCredential credential = null;

            var credentialList = Vault.RetrieveAll();
            if (credentialList.Count > 0)
            {
                credential = credentialList[0];
            }

            return credential;
        }
    }
}
