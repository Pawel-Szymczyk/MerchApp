using MerchApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MerchApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CartPage : Page
    {
        public CartViewModel ViewModel { get; }

        public CartPage()
        {
            InitializeComponent();
            ViewModel = App.Current.Services.GetRequiredService<CartViewModel>();

            ViewModel.RequestSubmitted += (_, _) =>
                DispatcherQueue.TryEnqueue(() => Frame.Navigate(typeof(ItemsPage)));

            ViewModel.ClearCartCommand.CanExecuteChanged += (_, _) =>
                DispatcherQueue.TryEnqueue(() => NavigateToCatalogue());
        }

        private void NavigateToCatalogue()
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
            else
                Frame.Navigate(typeof(ItemsPage));
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not int itemId) return;

            var cartItem = ViewModel.CartItems.FirstOrDefault(i => i.Item.Id == itemId);
            if (cartItem is null) return;

            ViewModel.RemoveItemCommand.Execute(cartItem);
        }

        private void BackToCatalogue_Click(object sender, RoutedEventArgs e)
        {
            // Wyczyść koszyk i wróć
            ViewModel.ClearCartCommand.Execute(null);
            Frame.Navigate(typeof(ItemsPage));
        }
    }
}
