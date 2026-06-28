
using MerchApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MerchApp.Services
{
    public class NotificationService : INotificationService
    {
        private readonly List<AppNotification> _notifications = new();

        public IReadOnlyList<AppNotification> Notifications => _notifications.AsReadOnly();
        public int UnreadCount => _notifications.Count(n => !n.IsRead);

        public event EventHandler? NotificationsChanged;

        public NotificationService()
        {
            try
            {
                Microsoft.Windows.AppNotifications.AppNotificationManager.Default.Register();
            }
            catch { }
        }

        public void Notify(string title, string message, NotificationKind kind, bool showToast = true)
        {
            var notification = new AppNotification
            {
                Title = title,
                Message = message,
                Kind = kind
            };

            _notifications.Insert(0, notification);
            NotificationsChanged?.Invoke(this, EventArgs.Empty);

            if (showToast)
                ShowToast(title, message);
        }

        public void MarkAllAsRead()
        {
            foreach (var n in _notifications)
                n.IsRead = true;

            NotificationsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Clear()
        {
            _notifications.Clear();
            NotificationsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void NotifyRequestApproved(string itemsSummary) =>
            Notify("Request approved",
                   $"Your items are ready for collection: {itemsSummary}",
                   NotificationKind.Success);

        public void NotifyRequestRejected(string reason) =>
            Notify("Request declined",
                   string.IsNullOrWhiteSpace(reason)
                       ? "Your rental request was declined by the manager."
                       : $"Your rental request was declined: {reason}",
                   NotificationKind.Error);

        public void NotifyReturnDueSoon(string itemName, DateTime dueDate) =>
            Notify("Return due soon",
                   $"{itemName}: return by {dueDate:d MMM yyyy}",
                   NotificationKind.Warning);

        public void NotifyNewRequest(string userName, string itemsSummary) =>
            Notify("New rental request",
                   $"{userName} is requesting: {itemsSummary}",
                   NotificationKind.Info);

        private static void ShowToast(string title, string message)
        {
            try
            {
                var builder = new Microsoft.Windows.AppNotifications.Builder.AppNotificationBuilder()
                    .AddText(title)
                    .AddText(message);

                Microsoft.Windows.AppNotifications.AppNotificationManager.Default.Show(builder.BuildNotification());
            }
            catch { }
        }
    }
}
