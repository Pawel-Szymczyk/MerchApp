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

        //private void RequestRow_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        //{
        //    if (sender is not Grid grid) return;
        //    if (grid.DataContext is not SelectableRequest selectable) return;

        //    ViewModel.ToggleExpandCommand.Execute(selectable);
        //}
        private void RequestRow_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (sender is not Border border) return;
            if (border.DataContext is not SelectableRequest selectable) return;

            ViewModel.ToggleExpandCommand.Execute(selectable);
        }

        private void Checkbox_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            // Zatrzymaj propagację — kliknięcie checkboxa nie powinno rozwijać detali
            e.Handled = true;
        }
    }
}
