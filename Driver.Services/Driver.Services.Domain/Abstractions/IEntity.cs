namespace Driver.Services.Domain.Abstractions;

public interface IEntity
{
    DateTimeOffset CreatedAt { get; }
    DateTimeOffset UpdatedAt { get; }
    bool Deleted { get; }
    DateTimeOffset? DeletedAt { get; }
}
