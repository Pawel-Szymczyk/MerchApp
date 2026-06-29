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
    /// <summary>
    /// Wraps RentalRequest with IsSelected for checkbox support.
    /// </summary>
    public partial class SelectableRequest : ObservableObject
    {
        public RentalRequest Request { get; }

        [ObservableProperty]
        private bool _isSelected;

        [ObservableProperty]
        private bool _isExpanded;

        public SelectableRequest(RentalRequest request)
        {
            Request = request;
        }
    }

    public partial class ManagerViewModel : ObservableObject
    {
        private readonly ISharePointService _sharePointService;
        private readonly INotificationService _notificationService;

        // -------------------------------------------------------------------------
        // Observable properties
        // -------------------------------------------------------------------------

        [ObservableProperty]
        private ObservableCollection<SelectableRequest> _requests = new();

        [ObservableProperty]
        private ObservableCollection<SelectableRequest> _filteredRequests = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private string _selectedFilter = "Pending";

        [ObservableProperty]
        private string _rejectReason = string.Empty;

        [ObservableProperty]
        private bool _allSelected;

        [ObservableProperty]
        private int _expandedRequestId = -1;

        // -------------------------------------------------------------------------

        public int SelectedCount => FilteredRequests.Count(r => r.IsSelected);
        public bool HasSelection => SelectedCount > 0;

        // Pokaż przyciski tylko gdy wszystkie zaznaczone są Pending
        public bool HasPendingSelection =>
            SelectedCount > 0 &&
            FilteredRequests
                .Where(r => r.IsSelected)
                .All(r => r.Request.Status == RentalStatus.Pending);

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

                Requests.Clear();
                foreach (var r in requests)
                {
                    var selectable = new SelectableRequest(r);
                    selectable.PropertyChanged += (_, _) =>
                    {
                        OnPropertyChanged(nameof(SelectedCount));
                        OnPropertyChanged(nameof(HasSelection));
                        OnPropertyChanged(nameof(PendingSelectedCount));
                        OnPropertyChanged(nameof(HasPendingSelection));
                    };
                    Requests.Add(selectable);
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
        private void ToggleSelectAll()
        {
            foreach (var r in FilteredRequests.Where(r => r.Request.Status == RentalStatus.Pending))
                r.IsSelected = AllSelected;

            OnPropertyChanged(nameof(SelectedCount));
            OnPropertyChanged(nameof(HasSelection));
        }

        [RelayCommand]
        private async Task ApproveSelectedAsync()
        {
            var selected = FilteredRequests
                .Where(r => r.IsSelected && r.Request.Status == RentalStatus.Pending)
                .ToList();

            if (!selected.Any()) return;

            IsBusy = true;

            try
            {
                foreach (var s in selected)
                {
                    await _sharePointService.ApproveRequestAsync(s.Request.Id);

                    var itemsSummary = string.Join(", ",
                        s.Request.Items.Select(i => $"{i.ItemName} ×{i.Quantity}"));
                    _notificationService.NotifyRequestApproved(itemsSummary);
                }

                RejectReason = string.Empty;
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
        private async Task RejectSelectedAsync()
        {
            var selected = FilteredRequests
                 .Where(r => r.IsSelected && r.Request.Status == RentalStatus.Pending)
                 .ToList();

            if (!selected.Any()) return;

            IsBusy = true;

            try
            {
                foreach (var s in selected)
                {
                    await _sharePointService.RejectRequestAsync(s.Request.Id, RejectReason);
                    _notificationService.NotifyRequestRejected(RejectReason);
                }

                RejectReason = string.Empty;
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
        private void ToggleExpand(SelectableRequest request)
        {
            if (request is null) return;

            var wasExpanded = request.IsExpanded;

            // Zwiń wszystkie
            foreach (var r in FilteredRequests)
                r.IsExpanded = false;

            // Rozwiń kliknięty jeśli był zwinięty
            if (!wasExpanded)
                request.IsExpanded = true;
        }


        // -------------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------------

        private void ApplyFilter()
        {
            var filtered = SelectedFilter switch
            {
                "Pending" => Requests.Where(r => r.Request.Status == RentalStatus.Pending),
                "Active" => Requests.Where(r => r.Request.Status == RentalStatus.Approved),
                "Overdue" => Requests.Where(r => r.Request.IsOverdue),
                "Returned" => Requests.Where(r => r.Request.Status == RentalStatus.Returned),
                _ => Requests.AsEnumerable()
            };

            FilteredRequests.Clear();
            foreach (var r in filtered)
                FilteredRequests.Add(r);

            AllSelected = false;
            OnPropertyChanged(nameof(SelectedCount));
            OnPropertyChanged(nameof(HasSelection));
        }

        partial void OnSelectedFilterChanged(string value) => ApplyFilter();

        partial void OnAllSelectedChanged(bool value)
        {
            foreach (var r in FilteredRequests)
                r.IsSelected = value;

            OnPropertyChanged(nameof(SelectedCount));
            OnPropertyChanged(nameof(HasSelection));
            OnPropertyChanged(nameof(PendingSelectedCount));
            OnPropertyChanged(nameof(HasPendingSelection));
        }

        public int PendingSelectedCount => FilteredRequests.Count(r => r.IsSelected && r.Request.Status == RentalStatus.Pending);
    }
}
