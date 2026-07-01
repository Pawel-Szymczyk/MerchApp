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

            // Pokaż URL SharePoint
            var settings = App.Current.Services.GetRequiredService<ISettingsService>();
            var uri = new Uri(settings.Settings.SharePoint.SiteUrl);
            SharePointUrlText.Text = $"{uri.Host} · {uri.AbsolutePath.Trim('/')}";
        }
    }
}
