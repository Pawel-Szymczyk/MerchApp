using MerchApp.Models;
using MerchApp.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text.Json;

namespace MerchApp.Services
{
    public class SettingsService : ISettingsService
    {
        //public AppSettings Settings { get; private set; }
        private AppSettings? _settings;

        private static string LocalConfigPath =>
            Path.Combine(
                Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                "MerchApp",
                "config.json");

        public AppSettings Settings => _settings ??= LoadSettings();

        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(Settings.SharePoint.SiteUrl) &&
            !string.IsNullOrWhiteSpace(Settings.SharePoint.ClientId) &&
            !string.IsNullOrWhiteSpace(Settings.SharePoint.TenantId) &&
            !string.IsNullOrWhiteSpace(Settings.Roles.ManagerEmail);

        private AppSettings LoadSettings()
        {
            //System.Diagnostics.Debug.WriteLine(LocalConfigPath);
            // Priority 1 — LocalAppData (production)
            if (File.Exists(LocalConfigPath))
            {
                try
                {
                    var json = File.ReadAllText(LocalConfigPath);
                    var settings = JsonSerializer.Deserialize(
                        json,
                        AppSettingsJsonContext.Default.AppSettings);
                    if (settings is not null) return settings;
                }
                catch { }
            }

            // Priority 2 — appsettings.json (development fallback)
            try
            {
                var devPath = Path.Combine(AppContext.BaseDirectory, "Config", "appsettings.json");
                if (File.Exists(devPath))
                {
                    var json = File.ReadAllText(devPath);
                    var settings = JsonSerializer.Deserialize(
                        json,
                        AppSettingsJsonContext.Default.AppSettings);
                    if (settings is not null) return settings;
                }
            }
            catch { }

            return new AppSettings();
        }

        public void SaveSettings(AppSettings settings)
        {
            _settings = settings;

            var dir = Path.GetDirectoryName(LocalConfigPath)!;
            Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(
                settings,
                AppSettingsJsonContext.Default.AppSettings);

            File.WriteAllText(LocalConfigPath, json);
        }
        //public SettingsService()
        //{
        //    var configPath = Path.Combine(
        //        AppContext.BaseDirectory, "Config", "appsettings.json");

        //    var config = new ConfigurationBuilder()
        //       .AddJsonFile(configPath, optional: false, reloadOnChange: false)
        //       .Build();

        //    Settings = new AppSettings();
        //    config.Bind(Settings);

        //    Validate();
        //}

        //private void Validate()
        //{
        //    var sp = Settings.SharePoint;

        //    if (string.IsNullOrWhiteSpace(sp.SiteUrl) || sp.SiteUrl.Contains("YOUR_TENANT"))
        //        throw new InvalidOperationException(
        //            "SharePoint SiteUrl is not configured. Please edit Config/appsettings.json.");

        //    if (string.IsNullOrWhiteSpace(sp.ClientId) || sp.ClientId.Contains("YOUR_AZURE"))
        //        throw new InvalidOperationException(
        //            "SharePoint ClientId is not configured. Please edit Config/appsettings.json.");

        //    if (string.IsNullOrWhiteSpace(sp.TenantId) || sp.TenantId.Contains("YOUR_AZURE"))
        //        throw new InvalidOperationException(
        //            "SharePoint TenantId is not configured. Please edit Config/appsettings.json.");

        //    if (string.IsNullOrWhiteSpace(Settings.Roles.ManagerEmail))
        //        throw new InvalidOperationException(
        //            "ManagerEmail is not configured. Please edit Config/appsettings.json.");
        //}
    }
}
