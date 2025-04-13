namespace Domain.Entities;

public class Quantity
{
    public int Available { get; private set; }
    public int Reserved { get; private set; }
    public int Total => Available + Reserved;

    public Quantity(int available, int reserved)
    {
        if (available < 0 || reserved < 0)
            //throw new DomainException("Stock values cannot be negative");
            throw new ArgumentOutOfRangeException(nameof(available));

        Available = available;
        Reserved = reserved;
    }
}
