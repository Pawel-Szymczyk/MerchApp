using MerchApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace MerchApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyRentalsPage : Page
    {
        public MyRentalsViewModel ViewModel { get; }

        public MyRentalsPage()
        {
            InitializeComponent();
            ViewModel = App.Current.Services.GetRequiredService<MyRentalsViewModel>();
            Loaded += async (_, _) => await ViewModel.LoadRequestsCommand.ExecuteAsync(null);
        }
    }
}
