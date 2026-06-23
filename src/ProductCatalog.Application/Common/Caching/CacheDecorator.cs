using ProductCatalog.Application.Common.Messaging;
using ZiggyCreatures.Caching.Fusion;

namespace ProductCatalog.Application.Common.Caching;

/// <summary>
/// Cache decorator that uses IFusionCache.
/// It resolves a key using ICacheKeyProvider{TRequest} and sets the cache entry with a tag based on the request type.
/// </summary>
public class CacheDecorator<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> inner, IFusionCache fusionCache,
    ICacheKeyProvider<TRequest> keyProvider, int durationSeconds) : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IRequestHandler<TRequest, TResponse> inner = inner ?? throw new ArgumentNullException(nameof(inner));
    private readonly IFusionCache fusionCache = fusionCache ?? throw new ArgumentNullException(nameof(fusionCache));
    private readonly ICacheKeyProvider<TRequest> keyProvider = keyProvider ?? throw new ArgumentNullException(nameof(keyProvider));

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default)
    {
        if (durationSeconds <= 0)
        {
            return await inner.Handle(request, cancellationToken);
        }

        var key = keyProvider.CreateKey(request!);
        var tag = request.GetType().FullName ?? request.GetType().Name;

        var entryOptions = new FusionCacheEntryOptions()
            .SetDuration(TimeSpan.FromSeconds(durationSeconds))
            .SetEagerRefresh(0.9f)     // example: eager refresh enabled
                                       //.SetTag(tag)              // tag the entry for invalidation by type
            .SetFailSafe(true);         // example: fail-safe enabled

        var value = await fusionCache.GetOrSetAsync<TResponse>(key, async ct =>
        {
            return await inner.Handle(request, ct);
        }, entryOptions, cancellationToken);

        return value;
    }
}