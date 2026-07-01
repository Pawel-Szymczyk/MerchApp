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

        private bool _isPaneOpen = true;

        public bool IsPaneOpen => _isPaneOpen;
        public bool IsPaneClosed => !_isPaneOpen;

        public ShellPage()
        {
            InitializeComponent();
            _session = App.Current.Services.GetRequiredService<ISessionContext>();

            SetupUserInfo();
            SetupNavigationForRole();

            NavView.PaneOpened += (_, _) =>
            {
                _isPaneOpen = true;
                UpdatePaneState();
            };
            NavView.PaneClosed += (_, _) =>
            {
                _isPaneOpen = false;
                UpdatePaneState();
            };
        }

        private void UpdatePaneState()
        {
            PaneExpanded.Visibility = _isPaneOpen ? Visibility.Visible : Visibility.Collapsed;
            PaneCollapsed.Visibility = _isPaneOpen ? Visibility.Collapsed : Visibility.Visible;
        }


        private void SetupUserInfo()
        {
            var user = _session.CurrentUser;
            if (user is null) return;

            UserNameText.Text = user.DisplayName;
            UserRoleText.Text = user.IsManager ? "Manager" : "User";

            var parts = user.DisplayName.Split(' ');
            var initials = parts.Length >= 2
                ? $"{parts[0][0]}{parts[1][0]}".ToUpper()
                : user.DisplayName[..1].ToUpper();

            AvatarInitials.Text = initials;
            AvatarInitialsSmall.Text = initials;

            var converter = new MerchApp.Converters.StringToColorBrushConverter();

            var fg = (Microsoft.UI.Xaml.Media.Brush)
                converter.Convert(user.AvatarColor, typeof(Microsoft.UI.Xaml.Media.Brush), null, string.Empty);
            var bg = (Microsoft.UI.Xaml.Media.Brush)
                converter.Convert(user.AvatarColorFaded, typeof(Microsoft.UI.Xaml.Media.Brush), null, string.Empty);

            AvatarBorder.Background = bg;
            AvatarInitials.Foreground = fg;
            AvatarBorderSmall.Background = bg;
            AvatarInitialsSmall.Foreground = fg;
        }

        private void SetupNavigationForRole()
        {
            if (_session.IsManager)
            {
                CatalogItem.Visibility = Visibility.Collapsed;
                MyRentalsItem.Visibility = Visibility.Collapsed;
                NotificationsItem.Visibility = Visibility.Collapsed;
                ManagerSeparator.Visibility = Visibility.Collapsed;
                RequestsItem.Visibility = Visibility.Visible;

                ContentFrame.Navigate(typeof(ManagerPage));
                NavView.SelectedItem = RequestsItem;
            }
            else
            {
                RequestsItem.Visibility = Visibility.Collapsed;
                ManagerSeparator.Visibility = Visibility.Collapsed;

                ContentFrame.Navigate(typeof(ItemsPage));
                NavView.SelectedItem = CatalogItem;
            }
        }

        private async void NavView_SelectionChanged(
            NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is not NavigationViewItem item) return;

            var tag = item.Tag?.ToString();

            var pageType = tag switch
            {
                "Catalogue" => typeof(ItemsPage),
                "MyRentals" => typeof(MyRentalsPage),
                "Notifications" => typeof(NotificationsPage),
                "Requests" => typeof(ManagerPage),
                _ => typeof(ItemsPage)
            };

            ContentFrame.BackStack.Clear();
            ContentFrame.Navigate(pageType);
        }

        private async void SignOut_Click(object sender, RoutedEventArgs e)
        {
            var auth = App.Current.Services.GetRequiredService<IAuthService>();
            var session = App.Current.Services.GetRequiredService<ISessionContext>();
            await auth.LogoutAsync();
            session.ClearUser();
        }

    }
}
