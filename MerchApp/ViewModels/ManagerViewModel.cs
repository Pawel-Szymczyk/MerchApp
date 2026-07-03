using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MerchApp.Models;
using MerchApp.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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

        private bool _suppressNotifications = false;

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

        public bool HasSelection => SelectedCount > 0;

        public bool HasReturnedSelection
        {
            get
            {
                try
                {
                    var snapshot = FilteredRequests.ToArray();
                    return snapshot.Length > 0 &&
                           snapshot.Where(r => r.IsSelected)
                                   .Any() &&
                           snapshot.Where(r => r.IsSelected)
                                   .All(r => r.Request.Status == RentalStatus.Returned ||
                                             r.Request.Status == RentalStatus.Rejected ||
                                             r.Request.IsOverdue);
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool HasPendingSelection
        {
            get
            {
                try
                {
                    var snapshot = FilteredRequests.ToArray();
                    return snapshot.Length > 0 &&
                           snapshot.Where(r => r.IsSelected).Any() &&
                           snapshot.Where(r => r.IsSelected)
                                   .All(r => r.Request.Status == RentalStatus.Pending);
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool HasApprovedSelection
        {
            get
            {
                try
                {
                    var snapshot = FilteredRequests.ToArray();
                    return snapshot.Length > 0 &&
                           snapshot.Where(r => r.IsSelected).Any() &&
                           snapshot.Where(r => r.IsSelected)
                                   .All(r => r.Request.Status == RentalStatus.Approved ||
                                             r.Request.IsOverdue);
                }
                catch
                {
                    return false;
                }
            }
        }

        public int SelectedCount
        {
            get
            {
                try { return FilteredRequests.ToArray().Count(r => r.IsSelected); }
                catch { return 0; }
            }
        }

        public int PendingSelectedCount
        {
            get
            {
                try { return FilteredRequests.ToArray().Count(r => r.IsSelected && r.Request.Status == RentalStatus.Pending); }
                catch { return 0; }
            }
        }

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
        private async Task LoadAsync() => await LoadInternalAsync();

        
        private async Task LoadInternalAsync(bool force = false)
        {
            if (IsBusy && !force) return;

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
                        if (_suppressNotifications) return;

                        OnPropertyChanged(nameof(SelectedCount));
                        OnPropertyChanged(nameof(HasSelection));
                        OnPropertyChanged(nameof(PendingSelectedCount));
                        OnPropertyChanged(nameof(HasPendingSelection));
                        OnPropertyChanged(nameof(HasApprovedSelection));
                        OnPropertyChanged(nameof(HasReturnedSelection));
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
                        s.Request.Items.Select(i => $"{i.ItemName}"));
                    _notificationService.NotifyRequestApproved(itemsSummary);
                }

                RejectReason = string.Empty;

                ClearSelection();
                await LoadInternalAsync(force: true);
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
                 .Where(r => r.IsSelected && (r.Request.Status == RentalStatus.Pending))
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

                ClearSelection();
                await LoadInternalAsync(force: true);
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

                ClearSelection();

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

            // collapse all
            foreach (var r in FilteredRequests)
                r.IsExpanded = false;

            // show the selected one if it was not expanded before
            if (!wasExpanded)
                request.IsExpanded = true;
        }

        [RelayCommand]
        private async Task MarkAsReturnedSelectedAsync()
        {
            var selected = FilteredRequests
                .Where(r => r.IsSelected &&
                       (r.Request.Status == RentalStatus.Approved || r.Request.IsOverdue))
                .ToList();

            if (!selected.Any()) return;

            IsBusy = true;

            try
            {
                foreach (var s in selected)
                    await _sharePointService.MarkAsReturnedAsync(s.Request.Id);

                ClearSelection();

                await LoadInternalAsync(force: true);
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
        private async Task RemoveSelectedAsync()
        {
            // Snapshot — copy before any changes
            var snapshot = FilteredRequests.ToArray();

            var selectedIds = snapshot
                .Where(r => r.IsSelected &&
                           (r.Request.Status == RentalStatus.Returned ||
                            r.Request.Status == RentalStatus.Rejected ||
                            r.Request.IsOverdue))
                .Select(r => r.Request.Id)
                .ToArray();

            if (!selectedIds.Any()) return;

            IsBusy = true;

            try
            {
                // clear selected without triggering PropertyChanged
                _suppressNotifications = true;
                foreach (var r in snapshot)
                    r.IsSelected = false;
                _suppressNotifications = false;

                // remove from SharePoint
                foreach (var id in selectedIds)
                    await _sharePointService.DeleteRentalRequestAsync(id);

                // refresh list
                await LoadInternalAsync(force: true);
            }
            catch (Exception ex)
            {
                _suppressNotifications = false;
                HasError = true;
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        // -------------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------------

        private void ApplyFilter()
        {
            var filtered = SelectedFilter switch
            {
                "Pending" => Requests.Where(r => r.Request.Status == RentalStatus.Pending),
                "Active" => Requests.Where(r => r.Request.Status == RentalStatus.Approved && !r.Request.IsOverdue),
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
            OnPropertyChanged(nameof(PendingSelectedCount));
            OnPropertyChanged(nameof(HasPendingSelection));
            OnPropertyChanged(nameof(HasApprovedSelection));
            OnPropertyChanged(nameof(HasReturnedSelection));
        }

        partial void OnSelectedFilterChanged(string value) => ApplyFilter();

        partial void OnAllSelectedChanged(bool value)
        {
            foreach (var r in FilteredRequests.ToList())
                r.IsSelected = value;

            OnPropertyChanged(nameof(SelectedCount));
            OnPropertyChanged(nameof(HasSelection));
            OnPropertyChanged(nameof(PendingSelectedCount));
            OnPropertyChanged(nameof(HasPendingSelection));
            OnPropertyChanged(nameof(HasApprovedSelection));
            OnPropertyChanged(nameof(HasReturnedSelection));
        }

      
        private void ClearSelection()
        {
            _suppressNotifications = true;

            foreach (var r in FilteredRequests.ToList())
                r.IsSelected = false;

            _suppressNotifications = false;

            AllSelected = false;
            OnPropertyChanged(nameof(SelectedCount));
            OnPropertyChanged(nameof(HasSelection));
            OnPropertyChanged(nameof(PendingSelectedCount));
            OnPropertyChanged(nameof(HasPendingSelection));
            OnPropertyChanged(nameof(HasApprovedSelection));
            OnPropertyChanged(nameof(HasReturnedSelection));
        }

    }
}
