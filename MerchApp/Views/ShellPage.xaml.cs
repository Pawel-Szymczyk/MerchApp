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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MerchApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellPage : Page
    {
        private readonly ISessionContext _session;

        public ShellPage()
        {
            InitializeComponent();

            _session = App.Current.Services.GetRequiredService<ISessionContext>();

            SetupUserInfo();
            SetupManagerItems();

            // Navigate to Catalogue by default
            ContentFrame.Navigate(typeof(ItemsPage));
            NavView.SelectedItem = CatalogItem;
        }

        private void SetupUserInfo()
        {
            var user = _session.CurrentUser;
            if (user is null) return;

            UserNameText.Text = user.DisplayName;
            UserRoleText.Text = user.IsManager ? "Manager" : "User";

            // Generate initials — e.g. "Jan Nowak" → "JN"
            var parts = user.DisplayName.Split(' ');
            AvatarInitials.Text = parts.Length >= 2
                ? $"{parts[0][0]}{parts[1][0]}".ToUpper()
                : user.DisplayName[..1].ToUpper();
        }

        private void SetupManagerItems()
        {
            if (!_session.IsManager) return;

            ManagerSeparator.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            RequestsItem.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            //DashboardItem.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            //InventoryItem.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }

        private void NavView_SelectionChanged(
            NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is not NavigationViewItem item) return;

            var tag = item.Tag?.ToString();

            //var pageType = tag switch
            //{
            //    "Catalogue" => typeof(ItemsPage),
            //    "MyRentals" => typeof(MyRentalsPage),
            //    "Notifications" => typeof(NotificationsPage),
            //    "Requests" => typeof(ManagerPage),
            //    "Dashboard" => typeof(ManagerPage),
            //    "Inventory" => typeof(InventoryPage),
            //    "Settings" => typeof(SettingsPage),
            //    _ => typeof(ItemsPage)
            //};
            var pageType = tag switch
            {
                "Catalogue" => typeof(ItemsPage),
                "MyRentals" => typeof(MyRentalsPage),
                "Notifications" => typeof(NotificationsPage),
                "Requests" => typeof(ManagerPage),
                _ => typeof(ItemsPage)
            };

            ContentFrame.Navigate(pageType);
        }
    }
}
