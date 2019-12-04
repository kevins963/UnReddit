using System.Threading.Tasks;
using Refit;

namespace UnReddit.RedditApi
{

    [Headers(new string[]
        {
        "User-Agent: MyRedditApp / 1.0.0",
        "Authorization: bearer"
        })]
    interface IRedditApi
    {
        [Get("/api/v1/me")]
        Task<UserId> GetMe();

    }
}
