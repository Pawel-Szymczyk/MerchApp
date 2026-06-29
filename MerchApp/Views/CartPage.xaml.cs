using MerchApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MerchApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CartPage : Page
    {
        public CartViewModel ViewModel { get; }

        //public CartPage()
        //{
        //    InitializeComponent();
        //    ViewModel = App.Current.Services.GetRequiredService<CartViewModel>();

        //    ViewModel.RequestSubmitted += (_, _) =>
        //        DispatcherQueue.TryEnqueue(() => Frame.Navigate(typeof(ItemsPage)));

        //    ViewModel.ClearCartCommand.CanExecuteChanged += (_, _) =>
        //        DispatcherQueue.TryEnqueue(() => NavigateToCatalogue());
        //}

        public CartPage()
        {
            InitializeComponent();
            ViewModel = App.Current.Services.GetRequiredService<CartViewModel>();

            ViewModel.RequestSubmitted += (_, _) =>
                DispatcherQueue.TryEnqueue(() => Frame.Navigate(typeof(ItemsPage)));

            Loaded += (_, _) => UpdateCalendarSelection();
        }

        private void NavigateToCatalogue()
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
            else
                Frame.Navigate(typeof(ItemsPage));
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;

            // Tag może być int lub long w zależności od WinUI 3
            int itemId;
            if (btn.Tag is int i)
                itemId = i;
            else if (btn.Tag is long l)
                itemId = (int)l;
            else
                return;

            var cartItem = ViewModel.CartItems.FirstOrDefault(c => c.Item.Id == itemId);
            if (cartItem is null) return;

            ViewModel.RemoveItemCommand.Execute(cartItem);
        }

        private void BackToCatalogue_Click(object sender, RoutedEventArgs e)
        {
            // Wyczyść koszyk i wróć
            ViewModel.ClearCartCommand.Execute(null);
            Frame.Navigate(typeof(ItemsPage));
        }

        private bool _selectingFrom = true;
        private bool _updatingCalendar = false;
        private void FromDate_Click(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            _selectingFrom = true;
            UpdateDateBorders(fromActive: true);

            // Przeskocz kalendarz do aktualnej daty odbioru
            if (ViewModel.RentalFrom != DateTimeOffset.MinValue)
                MainCalendar.SetDisplayDate(ViewModel.RentalFrom.DateTime);
        }

        private void ToDate_Click(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            _selectingFrom = false;
            UpdateDateBorders(fromActive: false);

            // Przeskocz kalendarz do aktualnej daty zwrotu
            if (ViewModel.RentalTo != DateTimeOffset.MinValue)
                MainCalendar.SetDisplayDate(ViewModel.RentalTo.DateTime);
        }


        private void UpdateDateBorders(bool fromActive)
        {
            var activeBrush = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["AccentFillColorDefaultBrush"];
            var inactiveBrush = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"];

            FromDateBorder.BorderBrush = fromActive ? activeBrush : inactiveBrush;
            ToDateBorder.BorderBrush = fromActive ? inactiveBrush : activeBrush;
        }

        private void Calendar_SelectedDatesChanged(
             CalendarView sender,
             CalendarViewSelectedDatesChangedEventArgs args)
        {
            // Ignoruj zmiany wywołane przez UpdateCalendarSelection
            if (_updatingCalendar) return;
            if (!args.AddedDates.Any()) return;

            var selected = new DateTimeOffset(args.AddedDates[0].Date, TimeSpan.Zero);

            if (_selectingFrom)
            {
                ViewModel.RentalFrom = selected;
                UpdateCalendarSelection();
            }
            else
            {
                if (selected <= ViewModel.RentalFrom)
                    return;

                ViewModel.RentalTo = selected;
                UpdateCalendarSelection();
            }
        }

        private void UpdateCalendarSelection()
        {
            _updatingCalendar = true;

            try
            {
                MainCalendar.SelectedDates.Clear();

                if (ViewModel.RentalFrom != DateTimeOffset.MinValue)
                    MainCalendar.SelectedDates.Add(ViewModel.RentalFrom);

                if (ViewModel.RentalTo != DateTimeOffset.MinValue)
                    MainCalendar.SelectedDates.Add(ViewModel.RentalTo);
            }
            finally
            {
                _updatingCalendar = false;
            }
        }
    }
}
