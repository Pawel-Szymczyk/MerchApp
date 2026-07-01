using MerchApp.Models;
using MerchApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MerchApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotificationsPage : Page
    {
        private readonly INotificationService _notificationService;
        private readonly ObservableCollection<AppNotification> _notifications = new();

        public NotificationsPage()
        {
            InitializeComponent();
            _notificationService = App.Current.Services
                .GetRequiredService<INotificationService>();

            NotificationsList.ItemsSource = _notifications;

            LoadNotifications();

            _notificationService.NotificationsChanged += OnNotificationsChanged;
            Unloaded += (_, _) => _notificationService.NotificationsChanged -= OnNotificationsChanged;
        }

        private void LoadNotifications()
        {
            _notifications.Clear();

            foreach (var n in _notificationService.Notifications)
                _notifications.Add(n);

            EmptyState.Visibility = _notifications.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;

            // Only mark as read if there are notifications
            if (_notificationService.Notifications.Count > 0)
                _notificationService.MarkAllAsRead();
        }

        private void OnNotificationsChanged(object? sender, System.EventArgs e)
            => DispatcherQueue.TryEnqueue(LoadNotifications);

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            if (_notificationService.Notifications.Count == 0) return;
            _notificationService.Clear();
        }
    }
}
