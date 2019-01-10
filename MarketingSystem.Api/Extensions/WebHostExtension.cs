using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MarketingSystem.Api.Extensions
{
    public interface IWebHostInitializer
    {
        Task InitAsync();
    }

    public static class WebHostExtension
    {
        public static IWebHost Initialize<TInit>(this IWebHost webHost) where TInit : class, IWebHostInitializer
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TInit>>();
                var initializer = services.GetService<TInit>();

                try
                {
                    logger.LogInformation($"Start initializing by {typeof(TInit).Name}");
                    initializer.InitAsync().Wait();
                    logger.LogInformation($"Initialized using {typeof(TInit).Name}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred while initialzing with {typeof(TInit).Name}");
                }
            }

            return webHost;
        }
    }
}