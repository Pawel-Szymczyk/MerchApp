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
        private ObservableCollection<AppNotification> _notifications = new();

        public NotificationsPage()
        {
            InitializeComponent();
            _notificationService = App.Current.Services
                .GetRequiredService<INotificationService>();

            LoadNotifications();

            _notificationService.NotificationsChanged += (_, _) =>
                DispatcherQueue.TryEnqueue(LoadNotifications);
        }

        private void LoadNotifications()
        {
            _notifications.Clear();
            foreach (var n in _notificationService.Notifications)
                _notifications.Add(n);

            NotificationsList.ItemsSource = _notifications;
            _notificationService.MarkAllAsRead();
        }

        private void ClearAll_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            _notificationService.Clear();
        }
    }
}
