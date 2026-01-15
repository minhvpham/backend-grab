using MediatR;
using System.Runtime.CompilerServices;

namespace Driver.Services.Domain.Abstractions;

public abstract class Entity
{
    public DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    public bool Deleted { get; protected set; } = false;
    public DateTimeOffset? DeletedAt { get; protected set; }
    public string? UpdateByUserId { get; private set; }
    public string? UpdateByIdentityName { get; private set; }

    private List<INotification>? _domainEvents;
    public IReadOnlyCollection<INotification>? DomainEvents => _domainEvents?.AsReadOnly();

    public void UpdateBy(string? userId, string? identityName)
    {
        UpdateByUserId = userId;
        UpdateByIdentityName = identityName;
    }

    public void UpdateCreatedAt(DateTimeOffset time) => CreatedAt = time;

    public void UpdateUpdatedAt(DateTimeOffset time) => UpdatedAt = time;

    public void Delete()
    {
        if (Deleted)
        {
            return;
        }

        Deleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents ??= new List<INotification>();
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        _domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }
}

public abstract class Entity<TKey> : Entity
    where TKey : IEquatable<TKey>
{
    private TKey? _id;
    public virtual TKey Id
    {
        get { return _id!; }
        protected set { _id = value; }
    }

    public bool IsTransient()
    {
        return EqualityComparer<TKey>.Default.Equals(Id, default);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not Entity<TKey>)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (GetType() != obj.GetType())
            return false;

        Entity<TKey> item = (Entity<TKey>)obj;

        if (item.IsTransient() || IsTransient())
            return false;
        else
            return item.Id.Equals(Id);
    }

    public override int GetHashCode()
    {
        if (!IsTransient() && Id is not null)
        {
            return Id.GetHashCode() ^ 31;
        }

        return RuntimeHelpers.GetHashCode(this);
    }
}
