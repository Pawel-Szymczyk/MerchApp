using MerchApp.Models;

namespace MerchApp.Services.Interfaces
{
    public interface ISettingsService
    {
        AppSettings Settings { get; }
        bool IsConfigured { get; }
        void SaveSettings(AppSettings settings);
    }
}
