using Microsoft.Windows.AppNotifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerchApp.Services
{
    public class NotificationService : INotificationService
    {
        public IReadOnlyList<AppNotification> Notifications => throw new NotImplementedException();

        public int UnreadCount => throw new NotImplementedException();

        public event EventHandler NotificationsChanged;

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void MarkAllAsRead()
        {
            throw new NotImplementedException();
        }

        public void Notify(string title, string message, NotificationKind kind, bool showToast = true)
        {
            throw new NotImplementedException();
        }
    }
}
