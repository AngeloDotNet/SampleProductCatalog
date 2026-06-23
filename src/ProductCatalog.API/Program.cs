
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ProductCatalog.API.Middleware;
using ProductCatalog.Application.Common.Caching;
using ProductCatalog.Application.Common.Messaging;
using ProductCatalog.Application.DependencyInjection;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Products.Commands.CreateProduct;
using ProductCatalog.Infrastructure.Messaging;
using ProductCatalog.Infrastructure.Messaging.RabbitMQ;
using ProductCatalog.Infrastructure.Persistence;
using ProductCatalog.Infrastructure.Repositories;

namespace ProductCatalog.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

        // Add services
        builder.Services.AddControllers();
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Database
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            options.LogTo(Console.WriteLine, [RelationalEventId.CommandExecuted]);

            options.EnableSensitiveDataLogging();
            options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));

            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        // Register explicit cache-key provider for GetProductQuery (compact key by Id)
        //builder.Services.AddSingleton<ICacheKeyProvider<GetProductQuery>, GetProductQueryCacheKeyProvider>();

        // Register open - generic default cache key provider so ICacheKeyProvider<T> can be resolved for any T
        builder.Services.AddSingleton(typeof(ICacheKeyProvider<>), typeof(GenericCacheKeyProvider<>));
        builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();

        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // Register custom mediator / dispatcher (this also registers memory cache, key store, invalidation service)
        builder.Services.AddSimpleMediator(typeof(CreateProductCommand).Assembly);
        builder.Services.AddFusionCacheServices(builder.Configuration);

        builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
        builder.Services.AddCors(options => options.AddPolicy("AllowAll", policy
            => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();
        app.UseCors("AllowAll");

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}