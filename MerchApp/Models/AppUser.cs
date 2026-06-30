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

        public string AvatarColor => GetAvatarColor(DisplayName);
        public string AvatarColorFaded => GetAvatarColorFaded(DisplayName);

        private static readonly string[] _colors = new[]
        {
            "#d06d1f", // orange (accent)
            "#2d6e8f", // blue
            "#5c8a3c", // green
            "#8f3c6e", // pink
            "#6e3c8f", // purple
            "#3c6e8f", // steel blue
            "#8f6e3c", // gold
            "#3c8f5c", // emerald
        };

        private static readonly string[] _colorsFaded = new[]
        {
            "#4d2a0b", // orange faded
            "#0f2a38", // blue faded
            "#1a2e10", // green faded
            "#2e1022", // pink faded
            "#1f1030", // purple faded
            "#0f2030", // steel blue faded
            "#2e2010", // gold faded
            "#0f2e1a", // emerald faded
        };

        private static string GetAvatarColor(string name)
        {
            var index = Math.Abs(name.GetHashCode()) % _colors.Length;
            return _colors[index];
        }

        private static string GetAvatarColorFaded(string name)
        {
            var index = Math.Abs(name.GetHashCode()) % _colorsFaded.Length;
            return _colorsFaded[index];
        }
    }
}
