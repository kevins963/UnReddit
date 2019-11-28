using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Refit;

namespace UnReddit
{
    class UserId
    {
        [JsonProperty("id")]
        public String ID;
    }

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
