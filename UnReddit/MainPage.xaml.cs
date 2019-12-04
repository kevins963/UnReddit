using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Refit;
using System.Text;
using System.Threading.Tasks;
using UnReddit.RedditApi;
using UnReddit.Services;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UnReddit
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private RedditService mRedditService = new RedditService("zGM8I2QfAiJYPA", "http://127.0.0.1:12345");
        private IRedditApi mRedditApi = null;
        public MainPage()
        {
            this.InitializeComponent();

            mRedditApi = RestService.For<IRedditApi>("https://oauth.reddit.com", new RefitSettings()
            {
                AuthorizationHeaderValueGetter = async () =>
                {
                    var token = await mRedditService.GetToken();
                    var result = Convert.ToBase64String(Encoding.ASCII.GetBytes(token));
                    return await Task.FromResult(token);
                },

            }); ;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var user = await mRedditApi.GetMe();
            Console.WriteLine(user.ID.ToString());
        }
    }
}
