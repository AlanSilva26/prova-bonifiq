using ProvaPub.Helpers.Interfaces;

namespace ProvaPub.Helpers;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
