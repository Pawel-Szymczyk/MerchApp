using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MerchApp.Services;
using MerchApp.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace MerchApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly ISessionContext _session;
        private readonly ISettingsService _settingsService;

        public event EventHandler? LoginSucceeded;
        public event EventHandler<string>? LoginFailed;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _statusMessage = "Sign in with your Microsoft 365 account to request merch items and track your rentals.";

        [ObservableProperty]
        private bool _isConnected;

        [ObservableProperty]
        private string _connectionUrl = string.Empty;

        public LoginViewModel(
            IAuthService authService, ISessionContext session, ISettingsService settingsService) 
        {
            _authService = authService;
            _session = session;
            _settingsService = settingsService;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            StatusMessage = "Connecting to Microsoft 365...";

            try
            {
                var user = await _authService.LoginAsync();
                _session.SetUser(user);

                StatusMessage = $"Welcome, {user.DisplayName}!";
                LoginSucceeded?.Invoke(this, EventArgs.Empty);
            }
            catch (Microsoft.Identity.Client.MsalException ex)
            {
                StatusMessage = "Sign-in was cancelled or failed.";
                LoginFailed?.Invoke(this, ex.Message);
            }
            catch (Exception ex)
            {
                StatusMessage = "An unexpected error occurred.";
                LoginFailed?.Invoke(this, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task CheckConnectionAsync()
        {
            try
            {
                var siteUrl = _settingsService.Settings.SharePoint.SiteUrl;

                // Simple HTTP check — no auth required
                using var client = new System.Net.Http.HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);

                var response = await client.GetAsync(siteUrl);

                IsConnected = response.IsSuccessStatusCode ||
                                response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                                response.StatusCode == System.Net.HttpStatusCode.Forbidden;
                ConnectionUrl = siteUrl;
            }
            catch
            {
                IsConnected = false;
                ConnectionUrl = string.Empty;
            }
        }
    }
}
