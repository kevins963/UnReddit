using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Security.Authentication.Web;
using Windows.Web.Http.Headers;
using Newtonsoft.Json;

namespace UnReddit.Services
{
    public class RedditService
    {
        private readonly HttpClient mHttpClient;
        private readonly AuthenticationManager mAuthenticationManager;
        private readonly string mClientId;
        private readonly string mRedirectUrl;

        public RedditService(string clientId, string redirectUrl)
        {
            mHttpClient = new HttpClient();
            mAuthenticationManager = new AuthenticationManager("reddit");
            mClientId = clientId;
            mRedirectUrl = redirectUrl;
        }

        public async Task<string> GetToken()
        {
            if (mAuthenticationManager.GetIsExpired())
            {
                await AuthorizeAsync();
            }

            return mAuthenticationManager.AccessToken;
        }

        public async Task AuthorizeAsync()
        {
            if (mAuthenticationManager.GetIsLoginRequired())
            {
                var authCode = await Authorize();
                await AccessToken(authCode);
            }
            else
            {
                await RefreshAccessToken(mAuthenticationManager.RefreshToken);
            }
        }

        async Task<string> Authorize()
        {
            var guid = System.Guid.NewGuid();
            var builder = new UriBuilder("https://www.reddit.com/api/v1/authorize.compact");
            var httpParams = System.Web.HttpUtility.ParseQueryString(builder.Query);
            httpParams["client_id"] = mClientId;
            httpParams["response_type"] = "code";
            httpParams["state"] = guid.ToString();
            httpParams["redirect_uri"] = mRedirectUrl;
            httpParams["duration"] = "permanent";
            httpParams["scope"] = "*";
            builder.Query = httpParams.ToString();

            var result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, builder.Uri, new Uri(mRedirectUrl));

            if (result.ResponseStatus != WebAuthenticationStatus.Success)
            {
                return string.Empty;
            }

            var resultParams = System.Web.HttpUtility.ParseQueryString(new Uri(result.ResponseData).Query);

            // TODO varify state value also

            return resultParams.Get("code");
        }

        async Task<bool> AccessToken(string authCode)
        {
            var builder = new UriBuilder("https://www.reddit.com/api/v1/access_token");
            var httpParams = System.Web.HttpUtility.ParseQueryString(builder.Query);
            httpParams["grant_type"] = "authorization_code";
            httpParams["code"] = authCode;
            httpParams["redirect_uri"] = "http://127.0.0.1:12345";
            builder.Query = httpParams.ToString();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, builder.Uri);
            request.Headers.Authorization = new HttpCredentialsHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(":")));
            request.Headers.UserAgent.ParseAdd("MyRedditApp/1.0.0");

            var result = await mHttpClient.SendRequestAsync(request);

            string data = await result.Content.ReadAsStringAsync();
            try
            {
                var authTokenRequest = JsonConvert.DeserializeObject<RedditApi.AuthTokenRequest>(data);
                mAuthenticationManager.SetRefreshToken(authTokenRequest.RefreshToken, authTokenRequest.AccessToken, authTokenRequest.ExpiresIn);

            }
            catch (JsonSerializationException ex)
            {
                Console.WriteLine(ex.Message);
                mAuthenticationManager.ResetToken();

                return false;
            }

            return true;
        }

        async Task<bool> RefreshAccessToken(string refreshToken)
        {
            var builder = new UriBuilder("https://www.reddit.com/api/v1/access_token");
            var httpParams = System.Web.HttpUtility.ParseQueryString(builder.Query);
            httpParams["grant_type"] = "refresh_token";
            httpParams["refresh_token"] = refreshToken;
            builder.Query = httpParams.ToString();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, builder.Uri);
            request.Headers.Authorization = new HttpCredentialsHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(":")));
            request.Headers.UserAgent.ParseAdd("MyRedditApp/1.0.0");

            var result = await mHttpClient.SendRequestAsync(request);

            string data = await result.Content.ReadAsStringAsync();
            try
            {
                var authTokenRequest = JsonConvert.DeserializeObject<RedditApi.AuthTokenRequest>(data);
                mAuthenticationManager.SetAccessToken(authTokenRequest.AccessToken, authTokenRequest.ExpiresIn);
            }
            catch (JsonSerializationException ex)
            {
                Console.WriteLine(ex.Message);

                // TODO: we do not always want to reset the token, we want to 
                // handle |invalid token| vs bad http request
                mAuthenticationManager.ResetToken();

                return false;
            }

            return true;
        }
    }
}
