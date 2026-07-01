namespace MerchApp.Models
{
    /// <summary>
    /// Strongly-typed representation of appsettings.json.
    /// </summary>
    public class AppSettings
    {
        public SharePointSettings SharePoint { get; set; } = new();
        public RolesSettings Roles { get; set; } = new();
    }

    public class SharePointSettings
    {
        public string SiteUrl { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string ItemsListName { get; set; } = "MerchItems";
        public string RentalRequestsListName { get; set; } = "RentalRequests";
        public string RentalItemsListName { get; set; } = "RentalItems";
    }

    public class RolesSettings
    {
        public string ManagerEmail { get; set; } = string.Empty;
    }
}
