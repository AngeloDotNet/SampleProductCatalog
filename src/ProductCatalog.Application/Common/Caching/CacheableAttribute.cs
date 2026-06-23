namespace ProductCatalog.Application.Common.Caching;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class CacheableAttribute : Attribute
{
    /// <summary>
    /// Cache duration in seconds.
    /// </summary>
    public int DurationSeconds { get; }

    public CacheableAttribute(int durationSeconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(durationSeconds);

        DurationSeconds = durationSeconds;
    }
}