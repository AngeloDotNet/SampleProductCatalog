using ZiggyCreatures.Caching.Fusion;

namespace ProductCatalog.Application.Common.Caching;

public class CacheInvalidationService(IFusionCache fusionCache) : ICacheInvalidationService
{
    public void InvalidateByPrefix(string prefix)
    {
        throw new NotSupportedException("InvalidateByPrefix is not supported in the FusionCache-based invalidation. Use tags or InvalidateKeys instead.");
    }

    public void InvalidateForRequestTypes(params Type[] requestTypes)
    {
        if (requestTypes == null)
        {
            return;
        }

        foreach (var t in requestTypes)
        {
            var tag = t.FullName ?? t.Name;

            try
            {
                fusionCache.RemoveByTag(tag);
            }
            catch
            {
                // swallow or log depending on policy
            }
        }
    }

    public void InvalidateKeys(IEnumerable<string> keys)
    {
        if (keys == null)
        {
            return;
        }

        foreach (var key in keys.Where(k => !string.IsNullOrWhiteSpace(k)))
        {
            try
            {
                fusionCache.Remove(key);
            }
            catch
            {
                // swallow or log
            }
        }
    }
}