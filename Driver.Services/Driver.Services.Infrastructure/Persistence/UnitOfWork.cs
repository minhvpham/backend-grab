using Driver.Services.Domain.Abstractions;
using MediatR;

namespace Driver.Services.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly DriverServicesDbContext _context;
    private readonly IMediator _mediator;

    public UnitOfWork(DriverServicesDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Dispatch domain events before saving
            await DispatchDomainEventsAsync(cancellationToken);

            var result = await _context.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        catch (Exception)
        {
            // Log exception if needed
            return false;
        }
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEntities = _context.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents!)
            .ToList();

        // Clear domain events to prevent duplicate processing
        foreach (var entity in domainEntities)
        {
            entity.Entity.ClearDomainEvents();
        }

        // Dispatch events using MediatR
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
