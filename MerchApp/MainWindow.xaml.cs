using MerchApp.Services;
using MerchApp.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

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

            var session = App.Current.Services.GetRequiredService<ISessionContext>();

            // navigate to login or shell depending on session state
            if(session.IsLoggedIn)
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
