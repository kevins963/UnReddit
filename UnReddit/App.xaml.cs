using MvvmCross.Platforms.Uap.Core;
using MvvmCross.Platforms.Uap.Views;
using MvvmCross.IoC;
using Windows.UI.Xaml.Controls;

namespace UnReddit
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App
    {
        public App()
        {
            InitializeComponent();
        }
    }

    public abstract class UnRedditApp : MvxApplication<MvxWindowsSetup<MyApp>, MyApp>
    {
    }
}
