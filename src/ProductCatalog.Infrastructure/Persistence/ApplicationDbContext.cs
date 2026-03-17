using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure.Messaging;

namespace ProductCatalog.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IEventBus eventBus) : DbContext(options), IUnitOfWork
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch domain events before saving
        var domainEvents = ChangeTracker.Entries<AggregateRoot>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        // Publish events to RabbitMQ after successful save
        foreach (var domainEvent in domainEvents)
        {
            await eventBus.PublishAsync(domainEvent, cancellationToken);
        }

        // Clear domain events
        ChangeTracker.Entries<AggregateRoot>().ToList().ForEach(e => e.Entity.ClearDomainEvents());

        return result;
    }
}