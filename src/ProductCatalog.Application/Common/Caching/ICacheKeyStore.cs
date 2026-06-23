namespace ProductCatalog.Application.Common.Caching;

/// <summary>
/// Tracks cache keys set by CacheDecorator so they can be invalidated later.
/// </summary>
public interface ICacheKeyStore
{
    void Add(string key);
    IEnumerable<string> GetAll();
    void Remove(string key);
}