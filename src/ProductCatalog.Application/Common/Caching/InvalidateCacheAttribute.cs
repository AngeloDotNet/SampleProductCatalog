namespace ProductCatalog.Application.Common.Caching;

/// <summary>
/// Apply on commands that modify data to instruct dispatcher to invalidate caches
/// for the specified query/request types.
/// Example: [InvalidateCache(typeof(GetProductQuery), typeof(GetAllProductsQuery))]
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class InvalidateCacheAttribute(params Type[] affectedRequestTypes) : Attribute
{
    public Type[] AffectedRequestTypes { get; } = affectedRequestTypes ?? Array.Empty<Type>();
}