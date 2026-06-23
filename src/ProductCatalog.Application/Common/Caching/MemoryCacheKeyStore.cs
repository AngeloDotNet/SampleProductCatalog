using System.Collections.Concurrent;

namespace ProductCatalog.Application.Common.Caching;

public class MemoryCacheKeyStore : ICacheKeyStore
{
    private readonly ConcurrentDictionary<string, byte> keys = new();

    public void Add(string key) => keys.TryAdd(key, 0);
    public IEnumerable<string> GetAll() => keys.Keys.ToList();
    public void Remove(string key) => keys.TryRemove(key, out _);
}