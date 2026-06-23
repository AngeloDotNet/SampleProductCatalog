namespace ProductCatalog.Application.Common.Caching;

/// <summary>
/// Strategy interface per generare la cache-key per una specifica request/query.
/// Implementa un provider per avere controllo fine-grained sulla key usata da CacheDecorator.
/// </summary>
public interface ICacheKeyProvider<TRequest>
{
    /// <summary>
    /// Crea la cache key per la request.
    /// </summary>
    string CreateKey(TRequest request);
}