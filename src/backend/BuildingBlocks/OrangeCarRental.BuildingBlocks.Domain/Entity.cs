namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

/// <summary>
/// Base class for entities.
/// Entities are defined by their identity, not their attributes.
/// </summary>
/// <typeparam name="TId">The type of the entity's identifier (must be a value object).</typeparam>
public abstract class Entity<TId> where TId : notnull
{
    /// <summary>
    /// The unique identifier for this entity.
    /// </summary>
    public TId Id { get; protected set; } = default!;

    protected Entity()
    {
    }

    protected Entity(TId id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity<TId>? a, Entity<TId>? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(Entity<TId>? a, Entity<TId>? b) => !(a == b);
}
