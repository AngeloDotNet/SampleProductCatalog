namespace ProductCatalog.Application.Common.Caching;

/// <summary>
/// Implement on commands that can deterministically produce the exact cache keys
/// to invalidate (fine-grained invalidation).
/// </summary>
public interface IInvalidateCacheKeys
{
    /// <summary>
    /// Return exact cache keys that should be removed after the command succeeds.
    /// Keys should match CacheKeyHelper.CreateCacheKey(...) used by CacheDecorator.
    /// </summary>
    IEnumerable<string> GetCacheKeysToInvalidate();
}