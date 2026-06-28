using System;
using System.Collections.Generic;
using System.Text;

namespace MerchApp.Models
{
    /// <summary>
    /// A single in-app notification entry shown in the Notifications page.
    /// </summary>
    public class AppNotification
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Title { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public NotificationKind Kind { get; init; } = NotificationKind.Info;
        public DateTime Timestamp { get; init; } = DateTime.Now;
        public bool IsRead { get; set; } = false;

        public string RelativeTime
        {
            get
            {
                var diff = DateTime.Now - Timestamp;
                return diff.TotalMinutes < 1 ? "just now"
                     : diff.TotalMinutes < 60 ? $"{(int)diff.TotalMinutes} min ago"
                     : diff.TotalHours < 24 ? $"{(int)diff.TotalHours} hr ago"
                     : diff.TotalDays < 2 ? "yesterday"
                     : Timestamp.ToString("d MMM");
            }
        }
    }
}
