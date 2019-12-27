using System.Threading.Tasks;
using Refit;

namespace UnReddit.RedditApi
{

    [Headers(new string[]
        {
        "User-Agent: MyRedditApp / 1.0.0",
        "Authorization: bearer"
        })]
    public interface IRedditApi
    {
        [Get("/api/v1/me")]
        Task<UserId> GetMe();


        [Get("/api/v1/me/karma")]
        Task<string> GetMeKarma();

        [Get("/api/v1/me/prefs")]
        Task<string> GetMePrefs();

        [Patch("/api/v1/me/prefs")]
        Task<string> SetMePrefs(/*prefs*/);
    }
}
