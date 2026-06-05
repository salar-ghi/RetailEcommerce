namespace Application.Services;

public static class FinanceMoney
{
    public static decimal Normalize(decimal amount, FinanceCurrency currency)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Monetary amounts must be non-negative. Use transaction direction for cash flow sign.");
        }

        var scale = currency == FinanceCurrency.IRR ? 0 : 2;
        return decimal.Round(amount, scale, MidpointRounding.AwayFromZero);
    }

    public static decimal NormalizeSigned(decimal amount, FinanceCurrency currency)
    {
        var scale = currency == FinanceCurrency.IRR ? 0 : 2;
        return decimal.Round(amount, scale, MidpointRounding.AwayFromZero);
    }
}
