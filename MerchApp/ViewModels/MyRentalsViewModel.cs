using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MerchApp.Models;
using MerchApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MerchApp.ViewModels
{
    public partial class MyRentalsViewModel : ObservableObject
    {
        private readonly ISharePointService _sharePointService;
        private readonly ISessionContext _session;
        private readonly INotificationService _notificationService;

        // -------------------------------------------------------------------------
        // Observable properties
        // -------------------------------------------------------------------------

        [ObservableProperty]
        private ObservableCollection<RentalRequest> _allRequests = new();

        [ObservableProperty]
        private ObservableCollection<RentalRequest> _filteredRequests = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isEmpty;

        [ObservableProperty]
        private string _selectedFilter = "All";

        // -------------------------------------------------------------------------

        public MyRentalsViewModel(
            ISharePointService sharePointService,
            ISessionContext session,
            INotificationService notificationService)
        {
            _sharePointService = sharePointService;
            _session = session;
            _notificationService = notificationService;
        }

        // -------------------------------------------------------------------------
        // Commands
        // -------------------------------------------------------------------------

        [RelayCommand]
        private async Task LoadRequestsAsync()
        {
            if (IsBusy) return;
            if (_session.CurrentUser is null) return;

            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            try
            {
                var requests = await _sharePointService
                    .GetMyRentalRequestsAsync(_session.CurrentUser.Email);

                AllRequests.Clear();
                foreach (var r in requests)
                    AllRequests.Add(r);

                ApplyFilter();
                CheckOverdueNotifications();
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
        private void FilterBy(string filter)
        {
            SelectedFilter = filter;
            ApplyFilter();
        }

        // -------------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------------

        private void ApplyFilter()
        {
            var filtered = SelectedFilter switch
            {
                "Pending" => AllRequests.Where(r => r.Status == RentalStatus.Pending),
                "Active" => AllRequests.Where(r => r.Status == RentalStatus.Approved),
                "Returned" => AllRequests.Where(r => r.Status == RentalStatus.Returned),
                _ => AllRequests.AsEnumerable()
            };

            FilteredRequests.Clear();
            foreach (var r in filtered)
                FilteredRequests.Add(r);

            IsEmpty = FilteredRequests.Count == 0;
        }

        /// <summary>
        /// Checks for overdue rentals and notifies user.
        /// </summary>
        private void CheckOverdueNotifications()
        {
            var overdue = AllRequests.Where(r => r.IsOverdue);

            foreach (var r in overdue)
            {
                var itemNames = string.Join(", ", r.Items.Select(i => i.ItemName));
                _notificationService.NotifyReturnDueSoon(itemNames, r.RentalTo);
            }
        }

        partial void OnSelectedFilterChanged(string value) => ApplyFilter();
    }
}
