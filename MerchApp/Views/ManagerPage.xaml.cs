using MerchApp.Models;
using MerchApp.ViewModels;
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
    public sealed partial class ManagerPage : Page
    {
        public ManagerViewModel ViewModel { get; }

        public ManagerPage()
        {
            InitializeComponent();
            ViewModel = App.Current.Services.GetRequiredService<ManagerViewModel>();
            Loaded += async (_, _) => await ViewModel.LoadCommand.ExecuteAsync(null);
        }

        private async void RequestItem_Click(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is not SelectableRequest selectable) return;

            var request = selectable.Request;

            var panel = new StackPanel { Spacing = 12 };

            // Items
            foreach (var item in request.Items)
                panel.Children.Add(new TextBlock
                {
                    Text = $"{item.ItemName}  ×{item.Quantity}"
                });

            // Dates
            panel.Children.Add(new TextBlock
            {
                Text = $"{request.RentalFrom:d MMM yyyy}  →  {request.RentalTo:d MMM yyyy}",
                Foreground = (Microsoft.UI.Xaml.Media.Brush)
                    Application.Current.Resources["TextFillColorSecondaryBrush"]
            });

            // Purpose
            if (!string.IsNullOrWhiteSpace(request.Purpose))
                panel.Children.Add(new TextBlock
                {
                    Text = $"Purpose: {request.Purpose}"
                });

            // Manager note
            if (!string.IsNullOrWhiteSpace(request.ManagerNote))
                panel.Children.Add(new TextBlock
                {
                    Text = $"Note: {request.ManagerNote}"
                });

            var dialog = new ContentDialog
            {
                Title = request.UserDisplayName,
                Content = panel,
                CloseButtonText = "Close",
                XamlRoot = XamlRoot
            };

            // Mark as returned button for approved requests
            if (request.Status == RentalStatus.Approved)
                dialog.PrimaryButtonText = "Mark as Returned";

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary &&
                request.Status == RentalStatus.Approved)
            {
                await ViewModel.MarkAsReturnedCommand.ExecuteAsync(request);
            }
        }
    }
}
