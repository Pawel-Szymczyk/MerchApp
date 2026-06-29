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

        private async void RequestsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RequestsList.SelectedItem is not RentalRequest request) return;

            var dialog = new ContentDialog
            {
                Title = $"Request — {request.UserDisplayName}",
                XamlRoot = XamlRoot,
                CloseButtonText = "Close"
            };

            var panel = new StackPanel { Spacing = 12 };

            // Items
            foreach (var item in request.Items)
            {
                panel.Children.Add(new TextBlock
                {
                    Text = $"{item.ItemName}  ×{item.Quantity}"
                });
            }

            // Dates
            panel.Children.Add(new TextBlock
            {
                Text = $"From: {request.RentalFrom:d MMM yyyy}  →  {request.RentalTo:d MMM yyyy}",
                Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
            });

            // Purpose
            if (!string.IsNullOrWhiteSpace(request.Purpose))
                panel.Children.Add(new TextBlock { Text = $"Purpose: {request.Purpose}" });

            // Manager note input
            var noteBox = new TextBox
            {
                PlaceholderText = "Note / reason (required for rejection)",
                Text = request.ManagerNote ?? string.Empty
            };
            panel.Children.Add(noteBox);

            dialog.Content = panel;

            // Buttons depending on status
            if (request.Status == RentalStatus.Pending)
            {
                dialog.PrimaryButtonText = "Approve";
                dialog.SecondaryButtonText = "Reject";
            }
            else if (request.Status == RentalStatus.Approved)
            {
                dialog.PrimaryButtonText = "Mark as Returned";
            }

            var result = await dialog.ShowAsync();

            ViewModel.ManagerNote = noteBox.Text;

            if (result == ContentDialogResult.Primary)
            {
                if (request.Status == RentalStatus.Pending)
                    await ViewModel.ApproveRequestCommand.ExecuteAsync(request);
                else if (request.Status == RentalStatus.Approved)
                    await ViewModel.MarkAsReturnedCommand.ExecuteAsync(request);
            }
            else if (result == ContentDialogResult.Secondary)
            {
                await ViewModel.RejectRequestCommand.ExecuteAsync(request);
            }

            RequestsList.SelectedItem = null;
        }
    }
}
