namespace ProductCatalog.Application.Common.Caching;

public class GenericCacheKeyProvider<TRequest> : ICacheKeyProvider<TRequest>
{
    public string CreateKey(TRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        return CacheKeyHelper.CreateCacheKey(request!);
    }
}