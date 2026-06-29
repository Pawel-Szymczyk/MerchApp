//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using MerchApp.Services;
//using Microsoft.Online.SharePoint.TenantAdministration;
//using Microsoft.SharePoint.Client;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace MerchApp.ViewModels
//{
//    public partial class SettingsViewModel : ObservableObject
//    {
//        private readonly ISettingsService _settingsService;
//        private readonly ISharePointService _sharePointService;
//        private readonly ISessionContext _session;

//        // -------------------------------------------------------------------------
//        // Observable properties
//        // -------------------------------------------------------------------------

//        [ObservableProperty]
//        private string _siteUrl = string.Empty;

//        [ObservableProperty]
//        private string _clientId = string.Empty;

//        [ObservableProperty]
//        private string _tenantId = string.Empty;

//        [ObservableProperty]
//        private string _itemsListName = string.Empty;

//        [ObservableProperty]
//        private string _rentalRequestsListName = string.Empty;

//        [ObservableProperty]
//        private string _rentalItemsListName = string.Empty;

//        [ObservableProperty]
//        private string _managerEmail = string.Empty;

//        [ObservableProperty]
//        private bool _isBusy;

//        [ObservableProperty]
//        private bool _hasError;

//        [ObservableProperty]
//        private string _errorMessage = string.Empty;

//        [ObservableProperty]
//        private bool _isTestSuccessful;

//        [ObservableProperty]
//        private string _testResultMessage = string.Empty;

//        [ObservableProperty]
//        private int _testItemCount;

//        // -------------------------------------------------------------------------

//        public SettingsViewModel(
//            ISettingsService settingsService,
//            ISharePointService sharePointService,
//            ISessionContext session)
//        {
//            _settingsService = settingsService;
//            _sharePointService = sharePointService;
//            _session = session;

//            LoadCurrentSettings();
//        }

//        // -------------------------------------------------------------------------
//        // Commands
//        // -------------------------------------------------------------------------

//        [RelayCommand]
//        private async Task TestConnectionAsync()
//        {
//            if (IsBusy) return;

//            IsBusy = true;
//            HasError = false;
//            IsTestSuccessful = false;
//            TestResultMessage = string.Empty;

//            try
//            {
//                var count = await _sharePointService.TestConnectionAsync();

//                IsTestSuccessful = true;
//                TestItemCount = count;
//                TestResultMessage = $"Connection successful · {count} items on list";
//            }
//            catch (Exception ex)
//            {
//                HasError = true;
//                ErrorMessage = ex.Message;
//                TestResultMessage = $"Connection failed: {ex.Message}";
//            }
//            finally
//            {
//                IsBusy = false;
//            }
//        }

//        // -------------------------------------------------------------------------
//        // Helpers
//        // -------------------------------------------------------------------------

//        private void LoadCurrentSettings()
//        {
//            var sp = _settingsService.Settings.SharePoint;

//            SiteUrl = sp.SiteUrl;
//            ClientId = sp.ClientId;
//            TenantId = sp.TenantId;
//            ItemsListName = sp.ItemsListName;
//            RentalRequestsListName = sp.RentalRequestsListName;
//            RentalItemsListName = sp.RentalItemsListName;
//            ManagerEmail = _settingsService.Settings.Roles.ManagerEmail;
//        }
//    }
//}
