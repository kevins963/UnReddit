using MvvmCross.Commands;
using MvvmCross.ViewModels;
using UnReddit.RedditApi;

namespace UnReddit.ViewModels
{
    public class TestViewModel : MvxViewModel
    {
        public IMvxAsyncCommand ConnectCommand { get; private set; }

        private IRedditApi mRedditApi = null;

        private UserId mUserId;
        public UserId UserId { get => mUserId; set => SetProperty(ref mUserId, value); }
        public string TestMe;


        public TestViewModel(IRedditApi redditApi)
        {
            mRedditApi = redditApi;
            ConnectCommand = new MvxAsyncCommand(async () => { UserId = await mRedditApi.GetMe();  });
            UserId = new UserId { ID = "???" };
            TestMe = "xyz";

        }
    }
}
