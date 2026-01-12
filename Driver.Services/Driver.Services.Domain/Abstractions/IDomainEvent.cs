using MediatR;

namespace Driver.Services.Domain.Abstractions;

public interface IDomainEvent : INotification
{
    DateTimeOffset OccurredOn { get; }
}
