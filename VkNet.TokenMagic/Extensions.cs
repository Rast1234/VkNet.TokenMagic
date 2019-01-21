using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using VkNet.Abstractions.Utils;
using VkNet.TokenMagic.Services;
using VkNet.TokenMagic.Services.Token.Google;
using VkNet.Utils;

namespace VkNet.TokenMagic
{
    public static class Extensions
    {
        public static IServiceCollection AddTokenMagic(this IServiceCollection services)
        {
            services.TryAddSingleton<RandomAppIdProvider>();
            services.TryAddSingleton<MTalkTcpClient>();
            services.TryAddSingleton<AndroidHttpClient>();
            services.TryAddSingleton<GoogleSecurityHttpClient>();
            services.TryAddSingleton<ReceiptReceiver>();

            services.TryAddSingleton<IRestClient, RestClientWithUserAgent>();  // vknet needs user-agent too

            services.AddHttpClient<AndroidHttpClient>().ConfigurePrimaryHttpMessageHandler(provider => MaybeProxyHttpHandler(provider)).SetHandlerLifetime(TimeSpan.FromMinutes(1));
            services.AddHttpClient<GoogleSecurityHttpClient>().ConfigurePrimaryHttpMessageHandler(provider => MaybeProxyHttpHandler(provider, true)).SetHandlerLifetime(TimeSpan.FromMinutes(1));
            services.AddHttpClient<IRestClient, RestClientWithUserAgent>().ConfigurePrimaryHttpMessageHandler(provider => MaybeProxyHttpHandler(provider)).SetHandlerLifetime(TimeSpan.FromMinutes(10));
            
            services.TryAddSingleton<IBrowser, BrowserWithAndroidToken>();

            return services;
        }

        public static HttpClientHandler MaybeProxyHttpHandler(IServiceProvider provider, bool ignoreSsl=false)
        {
            var proxy = provider.GetService<IWebProxy>();
            var logger = provider.GetService<ILogger<HttpClient>>();
            var useProxyCondition = proxy != null;
            if (useProxyCondition)
            {
                logger?.LogDebug($"Use Proxy: {proxy}");
            }

            Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> certCallback = null;
            if (ignoreSsl)
            {
                certCallback = (message, certificate2, arg3, arg4) => true;
                logger?.LogDebug($"Ignoring ssl");
            }
             
            return new HttpClientHandler
            {
                Proxy = proxy,
                UseProxy = useProxyCondition,
                ServerCertificateCustomValidationCallback = certCallback
            };
        }
    }
}
