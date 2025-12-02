using SFA.DAS.RoatpFinance.Web.Settings;

namespace SFA.DAS.RoatpFinance.Web.StartupExtensions
{
    public static class CacheStartupExtensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services, IWebConfiguration configuration, IHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    var redisConnectionString = configuration.SessionRedisConnectionString;
                    var sessionCachingDatabase = configuration.SessionCachingDatabase;
                    options.Configuration = $"{redisConnectionString},{sessionCachingDatabase}";
                });
            }

            return services;
        }
    }
}
