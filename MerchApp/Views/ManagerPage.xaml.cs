using MerchApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

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

        private void RequestRow_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (sender is not Border border) return;
            if (border.DataContext is not SelectableRequest selectable) return;

            ViewModel.ToggleExpandCommand.Execute(selectable);
        }

        private void Checkbox_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            // Stop propagation
            e.Handled = true;
        }
    }
}
