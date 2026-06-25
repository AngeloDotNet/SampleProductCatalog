using ProductCatalog.Domain.Exceptions;

namespace ProductCatalog.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency = "EUR")
    {
        if (amount < 0)
        {
            throw new DomainException("Amount cannot be negative");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new DomainException("Currency is required");
        }

        Amount = amount;
        Currency = currency.ToUpper();
    }
}