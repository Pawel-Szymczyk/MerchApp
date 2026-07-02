using MerchApp.Models;
using MerchApp.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MerchApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupPage : Page
    {
        private readonly ISettingsService _settingsService;

        public SetupPage()
        {
            InitializeComponent();
            _settingsService = App.Current.Services.GetRequiredService<ISettingsService>();

            // Pre-fill if values already exist
            SiteUrlBox.Text = _settingsService.Settings.SharePoint.SiteUrl ?? string.Empty;
            ClientIdBox.Text = _settingsService.Settings.SharePoint.ClientId ?? string.Empty;
            TenantIdBox.Text = _settingsService.Settings.SharePoint.TenantId ?? string.Empty;
            ManagerEmailBox.Text = _settingsService.Settings.Roles.ManagerEmail ?? string.Empty;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Visibility = Visibility.Collapsed;

            var siteUrl = SiteUrlBox.Text?.Trim() ?? string.Empty;
            var clientId = ClientIdBox.Text?.Trim() ?? string.Empty;
            var tenantId = TenantIdBox.Text?.Trim() ?? string.Empty;
            var managerEmail = ManagerEmailBox.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(siteUrl))
            { ShowError("SharePoint Site URL is required."); return; }

            if (!Uri.TryCreate(siteUrl, UriKind.Absolute, out _))
            { ShowError("SharePoint Site URL is not a valid URL."); return; }

            if (string.IsNullOrWhiteSpace(clientId))
            { ShowError("Entra App Client ID is required."); return; }

            if (!Guid.TryParse(clientId, out _))
            { ShowError("Client ID is not a valid GUID. Copy it exactly from Entra."); return; }

            if (string.IsNullOrWhiteSpace(tenantId))
            { ShowError("Entra Tenant ID is required."); return; }

            if (!Guid.TryParse(tenantId, out _))
            { ShowError("Tenant ID is not a valid GUID. Copy it exactly from Entra."); return; }

            if (string.IsNullOrWhiteSpace(managerEmail))
            { ShowError("Manager Email is required."); return; }

            if (!managerEmail.Contains('@'))
            { ShowError("Manager Email is not valid."); return; }

            // Save settings
            var settings = new AppSettings
            {
                SharePoint = new SharePointSettings
                {
                    SiteUrl = siteUrl,
                    ClientId = clientId,
                    TenantId = tenantId,
                    ItemsListName = "MerchItems",
                    RentalRequestsListName = "RentalRequests",
                    RentalItemsListName = "RentalItems"
                },
                Roles = new RolesSettings
                {
                    ManagerEmail = managerEmail
                }
            };

            _settingsService.SaveSettings(settings);

            var session = App.Current.Services.GetRequiredService<ISessionContext>();
            session.ClearUser();

            Frame.Navigate(typeof(LoginPage));
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}
