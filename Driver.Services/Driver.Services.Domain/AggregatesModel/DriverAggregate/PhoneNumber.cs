using Driver.Services.Domain.Exceptions;

namespace Driver.Services.Domain.AggregatesModel.DriverAggregate;

public class PhoneNumber
{
    public string Value { get; private set; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainValidationException("Phone number cannot be empty");

        // Basic phone number validation (can be enhanced)
        var cleanValue = value.Trim().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        
        if (cleanValue.Length < 10 || cleanValue.Length > 15)
            throw new DomainValidationException("Phone number must be between 10 and 15 digits");

        if (!cleanValue.All(char.IsDigit) && !cleanValue.StartsWith("+"))
            throw new DomainValidationException("Phone number must contain only digits (and optionally start with +)");

        Value = cleanValue;
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        if (obj is PhoneNumber other)
            return Value == other.Value;
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();
}
