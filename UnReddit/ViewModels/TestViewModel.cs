using MvvmCross.Commands;
using MvvmCross.ViewModels;
using UnReddit.RedditApi;

namespace UnReddit.ViewModels
{
    public class TestViewModel : MvxViewModel
    {
        public IMvxAsyncCommand ConnectCommand { get; private set; }
        public IMvxAsyncCommand KarmaCommand { get; private set; }
        public IMvxAsyncCommand PrefsCommand { get; private set; }

        private IRedditApi mRedditApi = null;

        private UserId mUserId;
        public UserId UserId { get => mUserId; set => SetProperty(ref mUserId, value); }

        public string mTest;
        public string Test { get => mTest; set => SetProperty(ref mTest, value);  }


        public TestViewModel(IRedditApi redditApi)
        {
            mRedditApi = redditApi;
            ConnectCommand = new MvxAsyncCommand(async () => { UserId = await mRedditApi.GetMe(); });
            KarmaCommand = new MvxAsyncCommand(async () => { Test = await mRedditApi.GetMeKarma(); });
            PrefsCommand = new MvxAsyncCommand(async () => { Test = await mRedditApi.GetMePrefs(); });
            UserId = new UserId { ID = "???" };
            Test = "xyz";

        }
    }
}
