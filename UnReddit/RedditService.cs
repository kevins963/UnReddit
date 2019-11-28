using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Web.Http;
using Windows.Security.Authentication.Web;
using Windows.Web.Http.Headers;
using Akavache;
using Newtonsoft.Json;
using Refit;

namespace UnReddit
{
    //"access_token": Your access token,
    //"token_type": "bearer",
    //"expires_in": Unix Epoch Seconds,
    //"scope": A scope string,
    //"refresh_token": Your refresh token

    class AuthTokenRequest
    {
        [JsonProperty("access_token")]
        public String AccessToken { get; set; }

        [JsonProperty("token_type")]
        public String TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("scope")]
        public String Scopes { get; set; }

        [JsonProperty("refresh_token")]
        public String RefreshToken { get; set; }
    }


    class RedditService
    {
        private const String StorageKeyAccessRefreshToken = "reddit_access_refesh_token";

        private HttpClient mHttpClient = new HttpClient();
        private Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private String mAccessToken = String.Empty;
        private DateTime mAccessTokenExpire = DateTime.Now;

        String GetKeyOrDefault(String key, String orDefault = "")
        {
            var val = localSettings.Values[key];

            if (val == null)
            {
                return orDefault;
            }

            return val.ToString();
        }
        public RedditService()
        {
            Akavache.BlobCache.ApplicationName = "UnReddit";
        }

        public async Task<string> GetToken()
        {
            if(string.IsNullOrEmpty(mAccessToken) || mAccessTokenExpire >= DateTime.Now)
            {
                await AuthorizeAsync();
            }

            return mAccessToken;
        }
        public async Task AuthorizeAsync()
        {
            var refreshToken = GetKeyOrDefault(StorageKeyAccessRefreshToken);

            if (String.IsNullOrEmpty(refreshToken))
            {
                var authCode = await Authorize();
                await AccessToken(authCode);

            }
            else
            {
                Console.Out.WriteLine("Found " + refreshToken);
                await RefreshAccessToken(refreshToken);
            }


        }

        async Task<string> Authorize()
        {
            var authUri = new Uri("https://www.reddit.com/api/v1/authorize.compact?client_id=&response_type=code&state=1234567890&redirect_uri=http://127.0.0.1:12345&duration=permanent&scope=*");

            var redirectUri = new Uri("http://127.0.0.1:12345");
            var result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, authUri, redirectUri);

            if (result.ResponseStatus != WebAuthenticationStatus.Success)
            {
                return String.Empty;
            }

            var httpParams = System.Web.HttpUtility.ParseQueryString(new Uri(result.ResponseData).Query);

            // TODO varify state value also

            return httpParams.Get("code");
        }

        async Task<bool> AccessToken(String authCode)
        {
            var builder = new UriBuilder("https://www.reddit.com/api/v1/access_token");
            var httpParams = System.Web.HttpUtility.ParseQueryString(builder.Query);
            httpParams["grant_type"] = "authorization_code";
            httpParams["code"] = authCode;
            httpParams["redirect_uri"] = "http://127.0.0.1:12345";
            builder.Query = httpParams.ToString();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, builder.Uri);
            request.Headers.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(":")));
            request.Headers.UserAgent.ParseAdd("MyRedditApp/1.0.0");

            var result = await mHttpClient.SendRequestAsync(request);

            String data = await result.Content.ReadAsStringAsync();
            try
            {
                var authTokenRequest = JsonConvert.DeserializeObject<AuthTokenRequest>(data);
                localSettings.Values[StorageKeyAccessRefreshToken] = authTokenRequest.RefreshToken;
                mAccessToken = authTokenRequest.AccessToken;
                mAccessTokenExpire = DateTime.Now.AddSeconds(authTokenRequest.ExpiresIn);
            }
            catch (Newtonsoft.Json.JsonSerializationException ex)
            {
                Console.WriteLine(ex.Message);

                localSettings.Values[StorageKeyAccessRefreshToken] = String.Empty;
                mAccessToken = String.Empty;
                mAccessTokenExpire = DateTime.Now;
                return false;
            }

            return true;
        }

        async Task<bool> RefreshAccessToken(String refreshToken)
        {
            var builder = new UriBuilder("https://www.reddit.com/api/v1/access_token");
            var httpParams = System.Web.HttpUtility.ParseQueryString(builder.Query);
            httpParams["grant_type"] = "refresh_token";
            httpParams["refresh_token"] = refreshToken;
            builder.Query = httpParams.ToString();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, builder.Uri);
            request.Headers.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(":")));
            request.Headers.UserAgent.ParseAdd("MyRedditApp/1.0.0");

            var result = await mHttpClient.SendRequestAsync(request);

            String data = await result.Content.ReadAsStringAsync();
            try
            {
                var authTokenRequest = JsonConvert.DeserializeObject<AuthTokenRequest>(data);
                // localSettings.Values[StorageKeyAccessRefreshToken] = authTokenRequest.RefreshToken;
                mAccessToken = authTokenRequest.AccessToken;
                mAccessTokenExpire = DateTime.Now.AddSeconds(authTokenRequest.ExpiresIn);
            }
            catch (Newtonsoft.Json.JsonSerializationException ex)
            {
                Console.WriteLine(ex.Message);

                localSettings.Values[StorageKeyAccessRefreshToken] = String.Empty;
                mAccessToken = String.Empty;
                mAccessTokenExpire = DateTime.Now;

                return false;
            }

            return true;
        }
    }
}
