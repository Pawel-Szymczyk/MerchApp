using MerchApp.Models;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerchApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IPublicClientApplication _msalClient;
        private readonly ISettingsService _settingsService;
        private readonly string[] _scopes;
        private AppUser? _currentUser;


        public AppUser? CurrentUser => _currentUser;
        public bool IsLoggedIn => _currentUser != null;


        public AuthService(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            var sp = _settingsService.Settings.SharePoint;

            _scopes = new[] { $"{sp.SiteUrl}/.default" };

            _msalClient = PublicClientApplicationBuilder
                .Create(sp.ClientId)
                .WithAuthority($"https://login.microsoftonline.com/{sp.TenantId}")
                .WithDefaultRedirectUri()
                .Build();

            EnableTokenCache();
        }

      

        public async Task<string> GetAccessTokenAsync()
        {
            if( _currentUser == null )
                throw new InvalidOperationException("User is not logged in.");

            var accounts = await _msalClient.GetAccountsAsync();
            var account = accounts.FirstOrDefault(a =>
                a.Username.Equals(_currentUser.Email, StringComparison.OrdinalIgnoreCase));

            if(account  == null )
                throw new InvalidOperationException("Cached account not found. Please log in again.");

            var result = await _msalClient
                .AcquireTokenSilent(_scopes, account)
                .ExecuteAsync();

            return result.AccessToken;
        }

        public async Task<AppUser?> LoginAsync()
        {
            AuthenticationResult result;

            try
            {
                var accounts = await _msalClient.GetAccountsAsync();
                var firstAccount = accounts.FirstOrDefault();

                if (firstAccount != null)
                {
                    result = await _msalClient
                        .AcquireTokenSilent(_scopes, firstAccount)
                        .ExecuteAsync();
                }
                else
                {
                    result = await _msalClient
                        .AcquireTokenInteractive(_scopes)
                        .WithPrompt(Prompt.SelectAccount)
                        .ExecuteAsync();
                }
            }
            catch (MsalUiRequiredException)
            {
                result = await _msalClient
                    .AcquireTokenInteractive(_scopes)
                    .WithPrompt(Prompt.SelectAccount)
                    .ExecuteAsync();
            }

            _currentUser = BuildUser(result);
            return _currentUser;
        }


        public async Task LogoutAsync()
        {
            var accounts = await _msalClient.GetAccountsAsync();
            foreach (var account in accounts)
            {
                await _msalClient.RemoveAsync(account);
            }

            _currentUser = null;
        }

        private AppUser? BuildUser(AuthenticationResult result)
        {
            var email = result.Account.Username;
            var managerEmail = _settingsService.Settings.Roles.ManagerEmail;

            var role = email.Equals(managerEmail, StringComparison.OrdinalIgnoreCase)
                ? UserRole.Manager 
                : UserRole.User;

            var displayName = result.ClaimsPrincipal?.FindFirst("name")?.Value ?? email;

            return new AppUser
            {
                Id = result.Account.HomeAccountId.Identifier,
                Email = email,
                DisplayName = displayName,
                Role = role
            };
        }

        private void EnableTokenCache()
        {
            var cacheDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MerchApp", "TokenCache");

            Directory.CreateDirectory(cacheDir);

            var storageProperties = new StorageCreationPropertiesBuilder(
                "msal_cache.bin", cacheDir)
                .Build();

            _ = Task.Run(async () =>
            {
                try
                {
                    var cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties);
                    cacheHelper.RegisterCache(_msalClient.UserTokenCache);
                }
                catch
                {
                }
            });
        }
    }
}
