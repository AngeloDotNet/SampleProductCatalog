namespace ProductCatalog.Application.Common.Caching;

public interface ICacheInvalidationService
{
    /// <summary>
    /// Invalidate all cache entries whose key starts with the provided prefix.
    /// </summary>
    void InvalidateByPrefix(string prefix);

    /// <summary>
    /// Invalidate keys for the given request/query types (prefix uses FullName).
    /// </summary>
    void InvalidateForRequestTypes(params Type[] requestTypes);

    /// <summary>
    /// Invalidate a set of exact keys (fine-grained).
    /// </summary>
    void InvalidateKeys(IEnumerable<string> keys);
}