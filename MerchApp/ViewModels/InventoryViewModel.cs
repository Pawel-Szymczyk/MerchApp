//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using MerchApp.Models;
//using MerchApp.Services;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Text;
//using System.Threading.Tasks;

//namespace MerchApp.ViewModels
//{
//    public partial class InventoryViewModel : ObservableObject
//    {
//        private readonly ISharePointService _sharePointService;

//        // -------------------------------------------------------------------------
//        // Observable properties
//        // -------------------------------------------------------------------------

//        [ObservableProperty]
//        private ObservableCollection<Item> _items = new();

//        [ObservableProperty]
//        private bool _isBusy;

//        [ObservableProperty]
//        private bool _hasError;

//        [ObservableProperty]
//        private string _errorMessage = string.Empty;

//        [ObservableProperty]
//        private string _newItemTitle = string.Empty;

//        [ObservableProperty]
//        private int _newItemCount = 1;

//        [ObservableProperty]
//        private bool _isAddingItem;

//        // -------------------------------------------------------------------------

//        public InventoryViewModel(ISharePointService sharePointService)
//        {
//            _sharePointService = sharePointService;
//        }

//        // -------------------------------------------------------------------------
//        // Commands
//        // -------------------------------------------------------------------------

//        [RelayCommand]
//        private async Task LoadItemsAsync()
//        {
//            if (IsBusy) return;

//            IsBusy = true;
//            HasError = false;
//            ErrorMessage = string.Empty;

//            try
//            {
//                var items = await _sharePointService.GetItemsAsync();

//                Items.Clear();
//                foreach (var item in items)
//                    Items.Add(item);
//            }
//            catch (Exception ex)
//            {
//                HasError = true;
//                ErrorMessage = ex.Message;
//            }
//            finally
//            {
//                IsBusy = false;
//            }
//        }

//        [RelayCommand]
//        private async Task UpdateCountAsync(Item item)
//        {
//            if (item is null) return;

//            try
//            {
//                await _sharePointService.UpdateItemCountAsync(item.Id, item.TotalCount);
//            }
//            catch (Exception ex)
//            {
//                HasError = true;
//                ErrorMessage = ex.Message;
//            }
//        }

//        [RelayCommand]
//        private async Task AddItemAsync()
//        {
//            if (string.IsNullOrWhiteSpace(NewItemTitle)) return;
//            if (NewItemCount < 1) return;

//            IsBusy = true;

//            try
//            {
//                await _sharePointService.AddItemAsync(NewItemTitle, NewItemCount);

//                // Reset form
//                NewItemTitle = string.Empty;
//                NewItemCount = 1;
//                IsAddingItem = false;

//                // Reload list
//                await LoadItemsAsync();
//            }
//            catch (Exception ex)
//            {
//                HasError = true;
//                ErrorMessage = ex.Message;
//            }
//            finally
//            {
//                IsBusy = false;
//            }
//        }

//        [RelayCommand]
//        private async Task DeleteItemAsync(Item item)
//        {
//            if (item is null) return;

//            IsBusy = true;

//            try
//            {
//                await _sharePointService.DeleteItemAsync(item.Id);
//                Items.Remove(item);
//            }
//            catch (Exception ex)
//            {
//                HasError = true;
//                ErrorMessage = ex.Message;
//            }
//            finally
//            {
//                IsBusy = false;
//            }
//        }

//        [RelayCommand]
//        private void ToggleAddItem()
//        {
//            IsAddingItem = !IsAddingItem;
//            NewItemTitle = string.Empty;
//            NewItemCount = 1;
//        }
//    }
//}
