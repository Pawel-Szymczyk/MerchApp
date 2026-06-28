using System;
using System.Collections.Generic;
using System.Text;

namespace MerchApp.Models
{
    /// <summary>
    /// Represents the currently logged-in Microsoft 365 user.
    /// Role is determined by matching email against ManagerEmail in appsettings.json.
    /// </summary>
    public class AppUser
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;

        public bool IsManager => Role == UserRole.Manager;
    }
}
