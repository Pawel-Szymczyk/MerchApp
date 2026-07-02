using MerchApp.Services.Interfaces;
using MerchApp.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Windows.Graphics;

namespace MerchApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            
            var appWindow = AppWindow;

            // Set window icon
            AppWindow.SetIcon("Assets/LogoDark/merch_title.ico");

            // Set window size
            appWindow.Resize(new SizeInt32(960, 850));

            // Ustaw ciemny title bar
            var titleBar = AppWindow.TitleBar;
            titleBar.ExtendsContentIntoTitleBar = false;



            var darkColor = new Windows.UI.Color { A = 255, R = 28, G = 28, B = 28 };      // #1c1c1c
            var accentColor = new Windows.UI.Color { A = 255, R = 208, G = 109, B = 31 };  // #d06d1f
            var textColor = new Windows.UI.Color { A = 255, R = 241, G = 241, B = 241 };   // #f1f1f1

            titleBar.BackgroundColor = darkColor;
            titleBar.ForegroundColor = textColor;
            titleBar.InactiveBackgroundColor = darkColor;
            titleBar.InactiveForegroundColor = textColor;
            titleBar.ButtonBackgroundColor = darkColor;
            titleBar.ButtonForegroundColor = textColor;
            titleBar.ButtonHoverBackgroundColor = accentColor;
            titleBar.ButtonHoverForegroundColor = textColor;
            titleBar.ButtonPressedBackgroundColor = accentColor;
            titleBar.ButtonPressedForegroundColor = textColor;
            titleBar.ButtonInactiveBackgroundColor = darkColor;
            titleBar.ButtonInactiveForegroundColor = textColor;

            var session = App.Current.Services.GetRequiredService<ISessionContext>();

            // navigate to login or shell depending on session state
            if (session.IsLoggedIn)
            {
                RootFrame.Navigate(typeof(ShellPage));
            }
            else
            {
                RootFrame.Navigate(typeof(LoginPage));
            }

            // react to login/logout
            session.UserChanged += (_, _) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    if (session.IsLoggedIn)
                        RootFrame.Navigate(typeof(ShellPage));
                    else
                        RootFrame.Navigate(typeof(LoginPage));
                });
            };
        }
    }
}
