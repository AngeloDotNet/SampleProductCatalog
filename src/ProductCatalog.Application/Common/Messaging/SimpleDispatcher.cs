using System.Collections;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Caching;
using ProductCatalog.Application.Common.Validation;

namespace ProductCatalog.Application.Common.Messaging;

/// <summary>
/// Dispatcher esteso per supportare ICacheKeyProvider{TRequest}.
/// Contiene l'extension AddSimpleMediator che registra:
/// - open-generic GenericCacheKeyProvider{T}
/// - ICacheKeyStore, ICacheInvalidationService
/// - handlers, validators, e crea CacheDecorator passando anche il provider specifico risolto da DI
/// </summary>
public class SimpleDispatcher(IServiceProvider provider) : IDispatcher
{
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        // 1) Run validators (IValidator<TRequest>), if any
        var validatorInterface = typeof(IValidator<>).MakeGenericType(requestType);
        var enumerableValidatorsType = typeof(IEnumerable<>).MakeGenericType(validatorInterface);

        if (provider.GetService(enumerableValidatorsType) is IEnumerable validatorsObj)
        {
            foreach (var rawValidator in validatorsObj)
            {
                dynamic validator = rawValidator!;
                ValidationResult result = await validator.ValidateAsync((dynamic)request, cancellationToken);

                if (!result.IsValid)
                {
                    throw new ValidationException(result.Errors);
                }
            }
        }

        // 2) Resolve handler and invoke
        var handlerInterface = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handler = provider.GetService(handlerInterface) ?? throw new InvalidOperationException($"No handler registered for {handlerInterface.FullName}");

        dynamic dHandler = handler;
        var response = await dHandler.Handle((dynamic)request, cancellationToken);

        // 3) After successful handler: perform cache invalidation (fine-grained or attribute-based)
        try
        {
            var invalidationService = provider.GetService<ICacheInvalidationService>();

            if (invalidationService != null)
            {
                // fine-grained invalidation (command can produce exact keys)
                if (request is IInvalidateCacheKeys fineGrained)
                {
                    var keys = fineGrained.GetCacheKeysToInvalidate();

                    if (keys != null)
                    {
                        var keyList = keys.Where(k => !string.IsNullOrWhiteSpace(k)).ToList();

                        if (keyList.Count > 0)
                        {
                            invalidationService.InvalidateKeys(keyList);
                        }
                    }
                }
                else
                {
                    // attribute-based invalidation by request types/prefix
                    if (requestType.GetCustomAttribute(typeof(InvalidateCacheAttribute)) is InvalidateCacheAttribute invalidateAttr
                        && invalidateAttr.AffectedRequestTypes != null && invalidateAttr.AffectedRequestTypes.Length > 0)
                    {
                        invalidationService.InvalidateForRequestTypes(invalidateAttr.AffectedRequestTypes);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            var logger = provider.GetService<ILogger<SimpleDispatcher>>();
            logger?.LogError(ex, "Error while invalidating cache for request {RequestType}", requestType.FullName);
        }

        return response;
    }
}