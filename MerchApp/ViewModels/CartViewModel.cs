using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MerchApp.Models;
using MerchApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerchApp.ViewModels
{
    public partial class CartViewModel : ObservableObject
    {
        private readonly ISharePointService _sharePointService;
        private readonly ICartService _cartService;
        private readonly ISessionContext _session;
        private readonly INotificationService _notificationService;

        // -------------------------------------------------------------------------
        // Observable properties
        // -------------------------------------------------------------------------

        [ObservableProperty]
        private ObservableCollection<CartItem> _cartItems = new();

        [ObservableProperty]
        private DateTimeOffset _rentalFrom = DateTimeOffset.Now;

        [ObservableProperty]
        private DateTimeOffset _rentalTo = DateTimeOffset.Now.AddDays(7);

        [ObservableProperty]
        private string _purpose = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isSubmitted;

        [ObservableProperty]
        private int _rentalDays;

        // -------------------------------------------------------------------------

        public bool IsEmpty => _cartService.IsEmpty;
        public bool IsDateRangeValid => _rentalTo > _rentalFrom;
        public int ItemCount => _cartService.Items.Count;

        public DateTimeOffset Today => DateTimeOffset.Now;

        public event EventHandler? RequestSubmitted;

        public CartViewModel(
            ISharePointService sharePointService,
            ICartService cartService,
            ISessionContext session,
            INotificationService notificationService)
        {
            _sharePointService = sharePointService;
            _cartService = cartService;
            _session = session;
            _notificationService = notificationService;

            // Sync cart items from service
            _cartService.CartChanged += OnCartChanged;

            RefreshCartItems();
            UpdateRentalDays();
        }

        // -------------------------------------------------------------------------
        // Commands
        // -------------------------------------------------------------------------

        [RelayCommand]
        private void RemoveItem(CartItem cartItem)
        {
            if (cartItem is null) return;
            _cartService.RemoveItem(cartItem.Item);
        }

        [RelayCommand]
        private void ClearCart()
        {
            _cartService.Clear();
        }

        [RelayCommand]
        private async Task SubmitRequestAsync()
        {
            if (IsBusy) return;
            if (_cartService.IsEmpty) return;
            if (!IsDateRangeValid) return;
            if (_session.CurrentUser is null) return;

            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            try
            {
                await _sharePointService.CreateRentalRequestAsync(
                    _session.CurrentUser,
                    _cartService.Items.ToList(),
                    RentalFrom.DateTime,
                    RentalTo.DateTime,
                    Purpose);

                // Notify manager
                var itemsSummary = string.Join(", ",
                    _cartService.Items.Select(i => i.Item.Title));

                _notificationService.NotifyNewRequest(
                    _session.CurrentUser.DisplayName,
                    itemsSummary);

                // Clear cart after successful submission
                _cartService.Clear();

                IsSubmitted = true;
                RequestSubmitted?.Invoke(this, EventArgs.Empty);
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

        // -------------------------------------------------------------------------
        // Property change handlers
        // -------------------------------------------------------------------------

        partial void OnRentalFromChanged(DateTimeOffset oldValue, DateTimeOffset newValue)
        {
            if (RentalTo <= newValue)
                RentalTo = newValue.AddDays(1);

            UpdateRentalDays();
            OnPropertyChanged(nameof(IsDateRangeValid));
            OnPropertyChanged(nameof(RentalFromDisplay));
        }

        partial void OnRentalToChanged(DateTimeOffset oldValue, DateTimeOffset newValue)
        {
            UpdateRentalDays();
            OnPropertyChanged(nameof(IsDateRangeValid));
            OnPropertyChanged(nameof(RentalToDisplay));
        }

        // -------------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------------

        private void OnCartChanged(object? sender, EventArgs e)
        {
            RefreshCartItems();
            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(ItemCount));
        }

        private void RefreshCartItems()
        {
            CartItems.Clear();
            foreach (var item in _cartService.Items)
                CartItems.Add(item);
        }

        private void UpdateRentalDays()
        {
            RentalDays = Math.Max(1, (RentalTo - RentalFrom).Days);
        }

        public string RentalFromDisplay =>
    RentalFrom == DateTimeOffset.MinValue
        ? "Select date"
        : RentalFrom.ToString("d MMM yyyy");

        public string RentalToDisplay =>
            RentalTo == DateTimeOffset.MinValue
                ? "Select date"
                : RentalTo.ToString("d MMM yyyy");
    }
}
