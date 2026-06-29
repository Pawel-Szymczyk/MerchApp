using MerchApp.Services;
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
using MerchApp.Services;


namespace MerchApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    //public sealed partial class ItemsPage : Page
    //{
    //    public ItemsViewModel ViewModel { get; }
    //    private readonly ICartService _cartService;

    //    public ItemsPage()
    //    {
    //        InitializeComponent();
    //        ViewModel = App.Current.Services.GetRequiredService<ItemsViewModel>();
    //        _cartService = App.Current.Services.GetRequiredService<ICartService>();

    //        ViewModel.NavigateToCart += OnNavigateToCart;
    //        _cartService.CartChanged += OnCartChanged;

    //        Loaded += async (_, _) => await ViewModel.LoadItemsCommand.ExecuteAsync(null);
    //        Unloaded += OnUnloaded;
    //    }

    //    private void OnNavigateToCart(object? sender, System.EventArgs e)
    //        => Frame.Navigate(typeof(CartPage));

    //    private void OnCartChanged(object? sender, System.EventArgs e)
    //        => DispatcherQueue.TryEnqueue(UpdateCartButton);

    //    private void OnUnloaded(object sender, RoutedEventArgs e)
    //    {
    //        ViewModel.Dispose();
    //        ViewModel.NavigateToCart -= OnNavigateToCart;
    //        _cartService.CartChanged -= OnCartChanged;
    //        Unloaded -= OnUnloaded;
    //    }

    //    private void UpdateCartButton()
    //    {
    //        var count = _cartService.TotalQuantity;
    //        CartCountText.Text = count > 0 ? count.ToString() : string.Empty;
    //    }

    //    private void AddItem_Click(object sender, RoutedEventArgs e)
    //    {
    //        if (sender is not Button btn) return;
    //        if (btn.Tag is not int itemId) return;

    //        var item = ViewModel.Items.FirstOrDefault(i => i.Id == itemId);
    //        if (item is null) return;

    //        ViewModel.AddToCartCommand.Execute(item);
    //        RefreshQuantity(btn, itemId);
    //    }

    //    private void RemoveItem_Click(object sender, RoutedEventArgs e)
    //    {
    //        if (sender is not Button btn) return;
    //        if (btn.Tag is not int itemId) return;

    //        var item = ViewModel.Items.FirstOrDefault(i => i.Id == itemId);
    //        if (item is null) return;

    //        ViewModel.RemoveFromCartCommand.Execute(item);
    //        RefreshQuantity(btn, itemId);
    //    }

    //    private void RefreshQuantity(Button btn, int itemId)
    //    {
    //        var parent = btn.Parent as Grid;
    //        if (parent is null) return;

    //        foreach (var child in parent.Children)
    //        {
    //            if (child is TextBlock tb && tb.Tag is int id && id == itemId)
    //            {
    //                var qty = ViewModel.GetCartQuantity(
    //                    ViewModel.Items.First(i => i.Id == itemId));
    //                tb.Text = qty > 0 ? qty.ToString() : string.Empty;
    //                break;
    //            }
    //        }
    //    }
    //}

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
