using System.Text.Json;

namespace ProductCatalog.Application.Common.Caching;

public static class CacheKeyHelper
{
    public static string CreateCacheKey(object request)
    {
        var typeName = request.GetType().FullName ?? request.GetType().Name;
        var payload = JsonSerializer.Serialize(request);

        return $"{typeName}:{payload}";
    }
}