using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Application.Common.Caching;
using ProductCatalog.Application.Common.Validation;
using ZiggyCreatures.Caching.Fusion;

namespace ProductCatalog.Application.Common.Messaging;

public static class SimpleDispatcherServiceExtensions
{
    ///// <summary>
    ///// Scans the provided assemblies and registers:
    ///// - IRequestHandler<TRequest,TResponse> implementations (handlers)
    ///// - IValidator<TRequest> implementations (validators)
    ///// - ICacheKeyProvider<TRequest> implementations (cache-key providers)
    ///// - memory cache, key store and invalidation service
    ///// - and configures CacheDecorator for requests marked with [Cacheable]
    ///// </summary>
    //public static IServiceCollection AddSimpleMediator(this IServiceCollection services, params Assembly[] assembliesToScan)
    //{
    //    if (assembliesToScan == null || assembliesToScan.Length == 0)
    //    {
    //        throw new ArgumentException("Provide at least one assembly to scan for handlers/validators.", nameof(assembliesToScan));
    //    }

    //    // Ensure infra services
    //    services.AddMemoryCache();
    //    services.AddSingleton<ICacheKeyStore, MemoryCacheKeyStore>();
    //    services.AddSingleton<ICacheInvalidationService, CacheInvalidationService>();

    //    // Fallback open-generic cache key provider
    //    services.AddSingleton(typeof(ICacheKeyProvider<>), typeof(GenericCacheKeyProvider<>));

    //    var handlerInterfaceType = typeof(IRequestHandler<,>);
    //    var validatorInterfaceType = typeof(IValidator<>);
    //    var cacheProviderInterfaceOpen = typeof(ICacheKeyProvider<>);
    //    var cacheableAttributeType = typeof(CacheableAttribute);

    //    // Collect types from assemblies (filter out compiler generated / system types)
    //    var allTypes = assembliesToScan.Distinct().SelectMany(a =>
    //    {
    //        try
    //        {
    //            return a.GetTypes();
    //        }
    //        catch (ReflectionTypeLoadException rex)
    //        {
    //            return rex.Types.Where(t => t != null)!;
    //        }
    //    }).Where(t => t != null && t.IsClass && !t.IsAbstract && !t.IsGenericType).ToList();

    //    // Register specific ICacheKeyProvider<T> implementations first (so they are available when creating decorators)
    //    var cacheProviderTypes = allTypes
    //        .SelectMany(t => t.GetInterfaces()
    //        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == cacheProviderInterfaceOpen)
    //        .Select(i => new { Impl = t, Interface = i })).ToList();

    //    foreach (var cp in cacheProviderTypes)
    //    {
    //        services.AddTransient(cp.Interface, cp.Impl);
    //    }

    //    // Register validators
    //    var validatorTypes = allTypes
    //        .SelectMany(t => t.GetInterfaces()
    //        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorInterfaceType)
    //        .Select(i => new { Impl = t, Interface = i })).ToList();

    //    foreach (var v in validatorTypes)
    //    {
    //        services.AddTransient(v.Interface, v.Impl);
    //    }

    //    // Now register handlers (and apply cache decorator where needed)
    //    var handlerTypes = allTypes
    //        .SelectMany(t => t.GetInterfaces()
    //        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
    //        .Select(i => new { Impl = t, Interface = i })).ToList();

    //    foreach (var h in handlerTypes)
    //    {
    //        var implType = h.Impl;
    //        var interfaceType = h.Interface;

    //        // register concrete type so it can be resolved in decorator factories
    //        services.AddTransient(implType);

    //        // get generic args: IRequestHandler<TRequest,TResponse>
    //        var genericArgs = interfaceType.GetGenericArguments();
    //        var requestType = genericArgs[0];
    //        var responseType = genericArgs[1];

    //        // if request is marked with [Cacheable], register decorator factory

    //        if (requestType.GetCustomAttribute(cacheableAttributeType) is CacheableAttribute cacheAttr)
    //        {
    //            // create factory to instantiate CacheDecorator<TRequest,TResponse>
    //            services.AddTransient(interfaceType, sp =>
    //            {
    //                // resolve inner concrete implementation
    //                var inner = sp.GetRequiredService(implType);

    //                // resolve IMemoryCache and key store
    //                var memoryCache = sp.GetRequiredService<IMemoryCache>();
    //                var keyStore = sp.GetRequiredService<ICacheKeyStore>();

    //                // resolve provider ICacheKeyProvider<TRequest> (will fall back to GenericCacheKeyProvider<TRequest>)
    //                var providerType = typeof(ICacheKeyProvider<>).MakeGenericType(requestType);
    //                var keyProvider = sp.GetRequiredService(providerType);

    //                // build decorator type
    //                var decoratorType = typeof(CacheDecorator<,>).MakeGenericType(requestType, responseType);

    //                // constructor expected: (IRequestHandler<TRequest,TResponse> inner, IMemoryCache cache, ICacheKeyStore keyStore, ICacheKeyProvider<TRequest> keyProvider, int durationSeconds)
    //                var decorator = Activator.CreateInstance(
    //                    decoratorType,
    //                    inner,
    //                    memoryCache,
    //                    keyStore,
    //                    keyProvider,
    //                    cacheAttr.DurationSeconds
    //                );

    //                return decorator!;
    //            });
    //        }
    //        else
    //        {
    //            // no caching: just map interface to concrete (resolve concrete when interface requested)
    //            services.AddTransient(interfaceType, sp => sp.GetRequiredService(implType));
    //        }
    //    }

    //    // Register dispatcher itself
    //    services.AddScoped<IDispatcher, SimpleDispatcher>();

    //    return services;
    //}

    /// <summary>
    /// Registers handlers, validators, cache-key providers and cache decorators.
    /// This version integrates with IFusionCache: the CacheDecorator factory resolves IFusionCache
    /// and an ICacheKeyProvider{TRequest} and instantiates CacheDecorator{TRequest,TResponse}.
    /// Also registers ICacheInvalidationService (if not already registered) and the generic cache-key provider fallback.
    /// Important: IDispatcher is registered as Scoped so handlers depending on scoped services (e.g. repositories / DbContext) can be resolved correctly.
    /// </summary>
    public static IServiceCollection AddSimpleMediator(this IServiceCollection services, params Assembly[] assembliesToScan)
    {
        if (assembliesToScan == null || assembliesToScan.Length == 0)
        {
            throw new ArgumentException("Provide at least one assembly to scan for handlers/validators.", nameof(assembliesToScan));
        }

        var handlerInterfaceType = typeof(IRequestHandler<,>);
        var validatorInterfaceType = typeof(IValidator<>);
        var cacheProviderInterfaceOpen = typeof(ICacheKeyProvider<>);
        var cacheableAttributeType = typeof(ProductCatalog.Application.Common.Caching.CacheableAttribute);

        // Ensure FusionCache infra is available in DI (do not register IFusionCache itself here;
        // the application should register FusionCache in Program.cs). Register invalidation service that uses IFusionCache.
        // Only register services if not already registered to avoid overwriting app registrations.
        if (!services.Any(s => s.ServiceType == typeof(ICacheInvalidationService)))
        {
            services.AddSingleton<ICacheInvalidationService, CacheInvalidationService>();
        }

        // Register fallback open-generic cache key provider if no specific provider registered
        if (!services.Any(s => s.ServiceType.IsGenericType && s.ServiceType.GetGenericTypeDefinition() == typeof(ICacheKeyProvider<>)))
        {
            services.AddSingleton(typeof(ICacheKeyProvider<>), typeof(GenericCacheKeyProvider<>));
        }

        // Collect concrete types from assemblies
        var allTypes = assembliesToScan.Distinct().SelectMany(a =>
        {
            try
            {
                return a.GetTypes();
            }
            catch (ReflectionTypeLoadException rex)
            {
                return rex.Types.Where(t => t != null)!;
            }
        }).Where(t => t != null && t.IsClass && !t.IsAbstract && !t.IsGenericType).ToList();

        // Register any concrete ICacheKeyProvider<T> implementations found in scanned assemblies
        var cacheProviderTypes = allTypes.SelectMany(t
            => t.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == cacheProviderInterfaceOpen)
                .Select(i => new { Impl = t, Interface = i })).ToList();

        foreach (var cp in cacheProviderTypes)
        {
            services.AddTransient(cp.Interface, cp.Impl);
        }

        // Register validators
        var validatorTypes = allTypes.SelectMany(t
            => t.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorInterfaceType)
                .Select(i => new { Impl = t, Interface = i })).ToList();

        foreach (var v in validatorTypes)
        {
            services.AddTransient(v.Interface, v.Impl);
        }

        // Register handlers and decorate with CacheDecorator when request type is annotated with [Cacheable]
        var handlerTypes = allTypes.SelectMany(t
            => t.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
                .Select(i => new { Impl = t, Interface = i })).ToList();

        foreach (var h in handlerTypes)
        {
            var implType = h.Impl;
            var interfaceType = h.Interface;

            // register concrete implementation so it can be resolved by factory
            services.AddTransient(implType);

            var genericArgs = interfaceType.GetGenericArguments();
            var requestType = genericArgs[0];
            var responseType = genericArgs[1];

            if (requestType.GetCustomAttribute(cacheableAttributeType) is CacheableAttribute cacheAttr)
            {
                // If request is marked Cacheable, register a factory that builds CacheDecorator<TRequest,TResponse>
                services.AddTransient(interfaceType, sp =>
                {
                    // resolve the concrete handler implementation in the current scope
                    var inner = sp.GetRequiredService(implType);

                    // resolve IFusionCache (must be registered in Program.cs)
                    var fusionCache = sp.GetRequiredService<IFusionCache>();

                    // resolve ICacheKeyProvider<TRequest> (falls back to GenericCacheKeyProvider<TRequest> if not overridden)
                    var providerType = typeof(ICacheKeyProvider<>).MakeGenericType(requestType);
                    var keyProvider = sp.GetRequiredService(providerType);

                    // create decorator type and instance
                    var decoratorType = typeof(CacheDecorator<,>).MakeGenericType(requestType, responseType);
                    var decorator = Activator.CreateInstance(decoratorType, inner, fusionCache, keyProvider, cacheAttr.DurationSeconds);

                    return decorator!;
                });
            }
            else
            {
                // not cacheable: map interface to concrete type (resolve concrete when interface requested)
                services.AddTransient(interfaceType, sp => sp.GetRequiredService(implType));
            }
        }

        // Register dispatcher as Scoped so it can resolve scoped handlers (DbContext, repositories, etc.)
        services.AddScoped<IDispatcher, SimpleDispatcher>();

        return services;
    }
}