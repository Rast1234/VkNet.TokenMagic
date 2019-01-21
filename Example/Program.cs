using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.NLog.Extensions.Logging;
using VkNet.NLog.Extensions.Logging.Extensions;
using VkNet.TokenMagic;
namespace Example
{
    class Program
    {
        private static void Main(string[] args)
        {
            
            var services = new ServiceCollection();
            services.AddTokenMagic();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageProperties = true,
                    CaptureMessageTemplates = true
                });
            });
            NLog.LogManager.LoadConfiguration("nlog.config");
            
            var vkNet = new VkApi(services);
            vkNet.Authorize(new ApiAuthParams
            {
                Login = "LOGIN",
                Password = "PASSWORD",
                Settings = Settings.Audio

            });

            var audios = vkNet.Audio.Get(new AudioGetParams
            {
                Count = 10
            });

            foreach (var audio in audios)
            {
                Console.WriteLine($"{audio.Artist} - {audio.Title} {audio.Url}");
            }
            Console.ReadLine();
            NLog.LogManager.Shutdown();
        }
    }

    
}
