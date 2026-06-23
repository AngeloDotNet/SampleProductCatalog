namespace ProductCatalog.Application.Common.Messaging;

/// <summary>
/// Marker interface for queries. Allows applying caching to queries only.
/// </summary>
public interface IQuery<TResponse> : IRequest<TResponse> { }