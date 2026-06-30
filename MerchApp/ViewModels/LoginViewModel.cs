using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MerchApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MerchApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly ISessionContext _session;

        public event EventHandler? LoginSucceeded;
        public event EventHandler<string>? LoginFailed;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _statusMessage = "Sign in with your Microsoft 365 account to request merch items and track your rentals.";

        public LoginViewModel(IAuthService authService, ISessionContext session)
        {
            _authService = authService;
            _session = session;
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
    }
}
