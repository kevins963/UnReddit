using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using UnReddit.ViewModels;

namespace UnReddit.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    [MvxViewFor(typeof(TestViewModel))]
    public sealed partial class TestView : MvxWindowsPage
    {
        public TestView()
        {
            this.InitializeComponent();
        }
    }
}
