using MerchApp.Services.Interfaces;
using MerchApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;

namespace MerchApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginViewModel ViewModel { get; }

        public LoginPage()
        {
            InitializeComponent();
            ViewModel = App.Current.Services.GetRequiredService<LoginViewModel>();

            try
            {
                var settings = App.Current.Services.GetRequiredService<ISettingsService>();
                var siteUrl = settings.Settings.SharePoint.SiteUrl;

                if (!string.IsNullOrWhiteSpace(siteUrl) && Uri.TryCreate(siteUrl, UriKind.Absolute, out var uri))
                {
                    SharePointUrlText.Text = $"{uri.Host} · {uri.AbsolutePath.Trim('/')}";
                }
                else
                {
                    SharePointUrlText.Text = "Not configured";
                }
            }
            catch
            {
                SharePointUrlText.Text = "Not configured";
            }

            Loaded += async (_, _) => await ViewModel.CheckConnectionAsync();
        }
    }
}
