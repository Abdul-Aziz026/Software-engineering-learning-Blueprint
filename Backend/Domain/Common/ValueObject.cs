namespace Domain.Common;

/// <summary>
/// Base class for Domain-Driven Design value objects.
/// A value object has NO identity — two instances are equal when all their
/// components are equal (unlike an entity, which is equal by Id).
/// Derived types list their components in <see cref="GetEqualityComponents"/>.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// The fields that define this value object's identity-by-value.
    /// Order matters and must be stable.
    /// </summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var component in GetEqualityComponents())
        {
            hash.Add(component);
        }
        return hash.ToHashCode();
    }

    public static bool operator ==(ValueObject? left, ValueObject? right) =>
        Equals(left, right);

    public static bool operator !=(ValueObject? left, ValueObject? right) =>
        !Equals(left, right);
}
