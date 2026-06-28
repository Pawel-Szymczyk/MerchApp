using MerchApp.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MerchApp.Services
{
    public class SettingsService : ISettingsService
    {
        public AppSettings Settings { get; private set; }

        public SettingsService()
        {
            var configPath = Path.Combine(
                AppContext.BaseDirectory, "Config", "appsettings.json");

            var config = new ConfigurationBuilder()
               .AddJsonFile(configPath, optional: false, reloadOnChange: false)
               .Build();

            Settings = new AppSettings();
            config.Bind(Settings);

            Validate();
        }

        private void Validate()
        {
            var sp = Settings.SharePoint;

            if (string.IsNullOrWhiteSpace(sp.SiteUrl) || sp.SiteUrl.Contains("YOUR_TENANT"))
                throw new InvalidOperationException(
                    "SharePoint SiteUrl is not configured. Please edit Config/appsettings.json.");

            if (string.IsNullOrWhiteSpace(sp.ClientId) || sp.ClientId.Contains("YOUR_AZURE"))
                throw new InvalidOperationException(
                    "SharePoint ClientId is not configured. Please edit Config/appsettings.json.");

            if (string.IsNullOrWhiteSpace(sp.TenantId) || sp.TenantId.Contains("YOUR_AZURE"))
                throw new InvalidOperationException(
                    "SharePoint TenantId is not configured. Please edit Config/appsettings.json.");

            if (string.IsNullOrWhiteSpace(Settings.Roles.ManagerEmail))
                throw new InvalidOperationException(
                    "ManagerEmail is not configured. Please edit Config/appsettings.json.");
        }
    }
}
