
using MerchApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Notifications;

namespace MerchApp.Services
{
    /// <summary>
    /// Handles in-app notifications and Windows toast notifications.
    /// In-app notifications are stored in memory and shown in the Notifications page.
    /// Toast notifications appear in the Windows notification centre.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>All in-app notifications, newest first.</summary>
        IReadOnlyList<AppNotification> Notifications { get; }

        /// <summary>Number of unread notifications.</summary>
        int UnreadCount { get; }

        /// <summary>Raised when a new notification is added.</summary>
        event EventHandler NotificationsChanged;

        /// <summary>Adds an in-app notification and optionally shows a Windows toast.</summary>
        void Notify(string title, string message, NotificationKind kind, bool showToast = true);

        /// <summary>Marks all notifications as read.</summary>
        void MarkAllAsRead();

        /// <summary>Clears all notifications.</summary>
        void Clear();
    }
}
