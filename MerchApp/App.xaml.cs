using MerchApp.Services;
using MerchApp.Services.Interfaces;
using MerchApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;

namespace MerchApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Services { get; }

        public static new App Current => (App)Application.Current;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Services = ConfigureServices();
            InitializeComponent();

            // set dark mode
            RequestedTheme = ApplicationTheme.Dark;
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var window = Services.GetRequiredService<MainWindow>();
            window.Activate();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Services
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<ISessionContext, SessionContext>();
            services.AddSingleton<ISharePointService, SharePointService>();
            services.AddSingleton<ICartService, CartService>();
            services.AddSingleton<INotificationService, NotificationService>();

            // ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<ItemsViewModel>();
            services.AddTransient<CartViewModel>();
            services.AddTransient<MyRentalsViewModel>();
            services.AddTransient<ManagerViewModel>();

            // Windows
            services.AddSingleton<MainWindow>();

            return services.BuildServiceProvider();
        }
    }
}
