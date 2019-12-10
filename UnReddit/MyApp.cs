using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.ViewModels;
using UnReddit.Services;
using UnReddit.RedditApi;
using Refit;
using System;
using System.Text;
using System.Threading.Tasks;
using UnReddit.ViewModels;

namespace UnReddit
{
    public class MyApp : MvxApplication
    {
        public override void Initialize()
        {
            Mvx.IoCProvider.RegisterSingleton(() => new RedditService("", "http://127.0.0.1:12345"));

            Mvx.IoCProvider.RegisterSingleton<IRedditApi>(() => RestService.For<IRedditApi>("https://oauth.reddit.com", new RefitSettings()
            {
                AuthorizationHeaderValueGetter = async () =>
                {
                    var redditService = Mvx.IoCProvider.GetSingleton<RedditService>();
                    var token = await redditService.GetToken();
                    var result = Convert.ToBase64String(Encoding.ASCII.GetBytes(token));
                    return await Task.FromResult(token);
                },

            }));

            RegisterAppStart<TestViewModel>();
        }
    }
    
}
