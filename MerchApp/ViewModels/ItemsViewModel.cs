using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MerchApp.Models;
using MerchApp.Services;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerchApp.ViewModels
{
    /// <summary>
    /// Wraps Item with IsSelected for checkbox support.
    /// </summary>
    public partial class SelectableItem : ObservableObject
    {
        public Item Item { get; }

        [ObservableProperty]
        private bool _isSelected;

        public SelectableItem(Item item)
        {
            Item = item;
        }
    }


    public partial class ItemsViewModel : ObservableObject, IDisposable
    {
        private readonly ISharePointService _sharePointService;
        private readonly ICartService _cartService;

        [ObservableProperty]
        private ObservableCollection<SelectableItem> _items = new();

        [ObservableProperty]
        private ObservableCollection<SelectableItem> _filteredItems = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private int _selectedCount;

        public bool HasSelection => SelectedCount > 0;
        public event EventHandler? NavigateToCart;
        public event EventHandler? CartChanged;

        public ItemsViewModel(
            ISharePointService sharePointService,
            ICartService cartService)
        {
            _sharePointService = sharePointService;
            _cartService = cartService;
            _cartService.CartChanged += OnCartChanged;
        }

        [RelayCommand]
        private async Task LoadItemsAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            try
            {
                var items = await _sharePointService.GetItemsAsync();

                Items.Clear();
                foreach (var item in items)
                {
                    var selectable = new SelectableItem(item);
                    selectable.PropertyChanged += (_, _) =>
                    {
                        // Sync with cart
                        if (selectable.IsSelected)
                            _cartService.AddItem(selectable.Item);
                        else
                            _cartService.RemoveItem(selectable.Item);

                        SelectedCount = _cartService.TotalCount;
                        OnPropertyChanged(nameof(HasSelection));
                    };
                    Items.Add(selectable);
                }

                ApplyFilter();
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void GoToCart()
        {
            NavigateToCart?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void Search(string query)
        {
            SearchQuery = query;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filtered = Items.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchQuery))
                filtered = filtered.Where(i =>
                    i.Item.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

            FilteredItems.Clear();
            foreach (var item in filtered)
                FilteredItems.Add(item);
        }

        private void OnCartChanged(object? sender, EventArgs e)
        {
            SelectedCount = _cartService.TotalCount;
            OnPropertyChanged(nameof(HasSelection));
            CartChanged?.Invoke(this, EventArgs.Empty);
        }

        partial void OnSearchQueryChanged(string value) => ApplyFilter();

        public void Dispose()
        {
            _cartService.CartChanged -= OnCartChanged;
        }
    }
}
