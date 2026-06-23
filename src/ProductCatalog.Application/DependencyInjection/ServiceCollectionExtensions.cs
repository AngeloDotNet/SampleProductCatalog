using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NeoSmart.Caching.Sqlite;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace ProductCatalog.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFusionCacheServices(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetSection("Redis").Value;

        services.AddLogging(b => b.SetMinimumLevel(LogLevel.Warning));
        services.AddFusionCache()
            .WithOptions(options =>
            {
                options.DefaultEntryOptions
                    //.SetDuration(TimeSpan.FromMinutes(10))
                    //.SetFailSafe(true)
                    .SetDuration(TimeSpan.FromMinutes(5))
                    .SetFailSafe(true, TimeSpan.FromHours(2), TimeSpan.FromMinutes(1))
                    .SetFactoryTimeouts(TimeSpan.FromMilliseconds(100))
                    .SetEagerRefresh(0.9f);

                // FACTORY SYNTHETIC TIMEOUTS: Debug (SO THEY WILL BE IGNORED)
                options.FactorySyntheticTimeoutsLogLevel = LogLevel.Debug;
                // ANY OTHER FACTORY ERRORS: Error (SO THEY WILL -NOT- BE IGNORED)
                options.FactoryErrorsLogLevel = LogLevel.Error;
            })
            .WithDefaultEntryOptions(new FusionCacheEntryOptions
            {
                //Duration = TimeSpan.FromMinutes(1),
                //IsFailSafeEnabled = true,
                //FailSafeMaxDuration = TimeSpan.FromHours(2),
                //FailSafeThrottleDuration = TimeSpan.FromSeconds(30),
                SkipMemoryCacheRead = true,
            })
            //.WithSerializer(new FusionCacheNewtonsoftJsonSerializer())
            .WithSerializer(new FusionCacheSystemTextJsonSerializer())
            .WithDistributedCache(new RedisCache(new RedisCacheOptions { Configuration = redisConnectionString }))
            .WithBackplane(new RedisBackplane(new RedisBackplaneOptions { Configuration = redisConnectionString }))
            .AsHybridCache();

        services.AddSqliteCache(options =>
        {
            options.CachePath = configuration.GetConnectionString("SqliteCache") ?? "Data Source=fusioncache.db";
            options.CleanupInterval = TimeSpan.FromMinutes(30);
        });

        //// FusionCache / Redis / SQLite setup
        //// Ensure you have connection strings: "Redis" and "SqliteCache" in appsettings.json
        //services.AddStackExchangeRedisCache(options =>
        //{
        //    options.Configuration = configuration.GetConnectionString("Redis"); // e.g. "localhost:6379"
        //    options.InstanceName = "ProductCatalog:";
        //});

        //// Register FusionCache SQLite disk provider
        //services.AddFusionCacheDistributedSqlite(options =>
        //{
        //    options.ConnectionString = builder.Configuration.GetConnectionString("SqliteCache") ?? "Data Source=fusioncache.db";
        //    options.TableName = "FusionCacheEntries";
        //});

        // Register FusionCache itself
        //services.AddFusionCache(options =>
        //{
        //    // global options (optional)
        //    //options.DefaultEntryOptions.SetDuration(TimeSpan.FromMinutes(10))
        //    //                            .SetFailSafe(true)
        //    //                           .SetEagerRefresh(0.9f);

        //    options.DefaultEntryOptions.SetDuration(TimeSpan.FromMinutes(10))
        //    .SetFailSafe(true)
        //    .SetFactoryTimeouts(TimeSpan.FromMilliseconds(100))
        //    .SetEagerRefresh(0.9f);
        //});
        //.WithDistributedCache(sp => sp.GetRequiredService<Microsoft.Extensions.Caching.IDistributedCache>()) // L2 (Redis)
        //.WithSerializer(new FusionCacheSystemTextJsonSerializer())
        //.WithBackplane(new FusionCacheBackplaneStackExchangeRedis(new FusionCacheBackplaneStackExchangeRedisOptions
        //{
        //    ConnectionString = configuration.GetConnectionString("Redis") // backplane uses same redis
        //}))
        //.WithDiskCache(sp => sp.GetRequiredService<IFusionCacheDistributedSqlite>()); // use sqlite disk cache

        return services;
    }
}