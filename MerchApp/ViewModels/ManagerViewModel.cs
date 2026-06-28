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
    public partial class ManagerViewModel : ObservableObject
    {
        private readonly ISharePointService _sharePointService;
        private readonly INotificationService _notificationService;

        // -------------------------------------------------------------------------
        // Observable properties
        // -------------------------------------------------------------------------

        [ObservableProperty]
        private ObservableCollection<RentalRequest> _allRequests = new();

        [ObservableProperty]
        private ObservableCollection<RentalRequest> _filteredRequests = new();

        [ObservableProperty]
        private RentalRequest? _selectedRequest;

        [ObservableProperty]
        private DashboardStats _stats = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private string _selectedFilter = "All";

        [ObservableProperty]
        private string _managerNote = string.Empty;

        // -------------------------------------------------------------------------

        public ManagerViewModel(
            ISharePointService sharePointService,
            INotificationService notificationService)
        {
            _sharePointService = sharePointService;
            _notificationService = notificationService;
        }

        // -------------------------------------------------------------------------
        // Commands
        // -------------------------------------------------------------------------

        [RelayCommand]
        private async Task LoadAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            try
            {
                var requests = await _sharePointService.GetAllRentalRequestsAsync();

                AllRequests.Clear();
                foreach (var r in requests)
                    AllRequests.Add(r);

                Stats = await _sharePointService.GetDashboardStatsAsync();

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
        private async Task ApproveRequestAsync(RentalRequest request)
        {
            if (request is null) return;

            IsBusy = true;

            try
            {
                await _sharePointService.ApproveRequestAsync(request.Id, ManagerNote);

                // Notify user
                var itemsSummary = string.Join(", ",
                    request.Items.Select(i => $"{i.ItemName} ×{i.Quantity}"));

                _notificationService.NotifyRequestApproved(itemsSummary);

                ManagerNote = string.Empty;

                // Reload
                await LoadAsync();
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
        private async Task RejectRequestAsync(RentalRequest request)
        {
            if (request is null) return;
            if (string.IsNullOrWhiteSpace(ManagerNote)) return;

            IsBusy = true;

            try
            {
                await _sharePointService.RejectRequestAsync(request.Id, ManagerNote);

                _notificationService.NotifyRequestRejected(ManagerNote);

                ManagerNote = string.Empty;

                await LoadAsync();
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
        private async Task MarkAsReturnedAsync(RentalRequest request)
        {
            if (request is null) return;

            IsBusy = true;

            try
            {
                await _sharePointService.MarkAsReturnedAsync(request.Id);
                await LoadAsync();
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

        [RelayCommand]
        private void SelectRequest(RentalRequest request)
        {
            SelectedRequest = request;
            ManagerNote = string.Empty;
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
                "Overdue" => AllRequests.Where(r => r.IsOverdue),
                "Returned" => AllRequests.Where(r => r.Status == RentalStatus.Returned),
                _ => AllRequests.AsEnumerable()
            };

            FilteredRequests.Clear();
            foreach (var r in filtered)
                FilteredRequests.Add(r);
        }

        partial void OnSelectedFilterChanged(string value) => ApplyFilter();
    }
}
