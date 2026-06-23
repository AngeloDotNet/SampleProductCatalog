using System.Text.Json;
using ProductCatalog.Application.Queries.GetProduct;

namespace ProductCatalog.Application.Common.Caching;

public class GetProductQueryCacheKeyProvider : ICacheKeyProvider<GetProductQuery>
{
    public string CreateKey(GetProductQuery request)
    {
        var typeName = typeof(GetProductQuery).FullName ?? typeof(GetProductQuery).Name;

        if (request is null)
        {
            return typeName;
        }

        var payload = JsonSerializer.Serialize(request);

        if (string.IsNullOrWhiteSpace(payload) || payload == "{}")
        {
            return typeName;
        }

        return $"{typeName}:{payload}";
    }
}