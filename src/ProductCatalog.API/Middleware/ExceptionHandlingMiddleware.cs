using System.Text.Json;
using ProductCatalog.Application.Common.Validation;

namespace ProductCatalog.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException vex)
        {
            logger.LogWarning(vex, "Validation error");

            // richer response shape for validation
            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                traceId = context.TraceIdentifier,
                status = context.Response.StatusCode,
                title = "Validation Failed",
                errors = vex.Errors.Select(e => new { field = e.Field, message = e.Message })
            };

            var json = JsonSerializer.Serialize(payload);
            await context.Response.WriteAsync(json);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                traceId = context.TraceIdentifier,
                status = context.Response.StatusCode,
                title = "An unexpected error occurred."
            };

            var json = JsonSerializer.Serialize(payload);
            await context.Response.WriteAsync(json);
        }
    }
}