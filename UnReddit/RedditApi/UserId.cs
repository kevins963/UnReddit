using Newtonsoft.Json;

namespace UnReddit.RedditApi
{
    public class UserId
    {
        [JsonProperty("id")]
        public string ID { get; set; }
    }
}
