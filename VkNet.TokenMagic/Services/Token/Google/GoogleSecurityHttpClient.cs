using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VkNet.TokenMagic.Models.Google;

namespace VkNet.TokenMagic.Services.Token.Google
{
    public class GoogleSecurityHttpClient {

        protected readonly HttpClient HttpClient;
        private readonly string _appId;
        private readonly ILogger<GoogleSecurityHttpClient> _logger;

        public GoogleSecurityHttpClient(HttpClient httpClient, RandomAppIdProvider appIdProvider, ILogger<GoogleSecurityHttpClient> logger)
        {
            HttpClient = httpClient;
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd(GcmUserAgent);
            _appId = appIdProvider.AppId;
            _logger = logger;
        }

        public async Task<string> GetReceipt(GoogleCredentials credentials)
        {

            await RequestReceipt1(credentials).ConfigureAwait(false);
            return await RequestReceipt2(credentials).ConfigureAwait(false);
        }

        private async Task RequestReceipt1(GoogleCredentials credentials)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, Url)
            {
                Method = HttpMethod.Post
            };
            request.Headers.Add("Authorization", $"AidLogin {credentials.Id}:{credentials.Token}");
            request.Content = new FormUrlEncodedContent(GetRequestParams(credentials));

            _logger?.LogDebug($"{nameof(RequestReceipt1)}");
            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        private async Task<string> RequestReceipt2(GoogleCredentials credentials)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, Url)
            {
                Method = HttpMethod.Post
            };
            request.Headers.Add("Authorization", $"AidLogin {credentials.Id}:{credentials.Token}");
            var requestParams = GetRequestParams(credentials);
            requestParams["X-scope"] = $"id{string.Empty}";  // id is always empty here?
            requestParams["X-kid"] = "|ID|2|";
            requestParams["X-X-kid"] = "|ID|2|";
            request.Content = new FormUrlEncodedContent(requestParams);
            _logger?.LogDebug($"{nameof(RequestReceipt2)}");
            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = content.Split(new[] {"|ID|2|:"}, StringSplitOptions.None)[1];
            if (result == "PHONE_REGISTRATION_ERROR")
            {
                throw new InvalidOperationException($"{nameof(RequestReceipt2)} bad response: {result}\n{content}");
            }

            return result;
        }

        protected Dictionary<string, string> GetRequestParams(GoogleCredentials credentials)
        {
            return new Dictionary<string, string>
            {
                {"X-scope", "GCM"},
                {"X-osv", "23"},
                {"X-subtype", "54740537194"},
                {"X-app_ver", "443"},
                {"X-kid", "|ID|1|"},
                {"X-appid", _appId},
                {"X-gmsv", "13283005"},
                {"X-cliv", "iid-10084000"},
                {"X-app_ver_name", "51.2 lite"},
                {"X-X-kid", "|ID|1|"},
                {"X-subscription", "54740537194"},
                {"X-X-subscription", "54740537194"},
                {"X-X-subtype", "54740537194"},
                {"app", "com.perm.kate_new_6"},
                {"sender", "54740537194"},
                {"device", credentials.Id.ToString()},
                {"cert", "966882ba564c2619d55d0a9afd4327a38c327456"},
                {"app_ver", "443"},
                {"info", "g57d5w1C4CcRUO6eTSP7b7VoT8yTYhY"},
                {"gcm_ver", "13283005"},
                {"plat", "0"},
                {"X-messenger2", "1"}
            };
        }

        protected const string GcmUserAgent = "Android-GCM/1.5 (generic_x86 KK)";

        protected static Uri Url = new Uri("https://android.clients.google.com/c2dm/register3");
    }
}