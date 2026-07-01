using MerchApp.Services.Interfaces;
using MerchApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


namespace MerchApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ItemsPage : Page
    {
        public ItemsViewModel ViewModel { get; }
        private readonly ICartService _cartService;

        public ItemsPage()
        {
            InitializeComponent();
            ViewModel = App.Current.Services.GetRequiredService<ItemsViewModel>();
            _cartService = App.Current.Services.GetRequiredService<ICartService>();

            ViewModel.NavigateToCart += OnNavigateToCart;
            Loaded += async (_, _) => await ViewModel.LoadItemsCommand.ExecuteAsync(null);
            Unloaded += OnUnloaded;
        }

        private void OnNavigateToCart(object? sender, System.EventArgs e)
            => Frame.Navigate(typeof(CartPage));

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Dispose();
            ViewModel.NavigateToCart -= OnNavigateToCart;
            Unloaded -= OnUnloaded;
        }
    }
}
