namespace ApplicationLayer.Common.Enums;

public sealed class TimeframeUnit
{
    public string Name { get; }
    public int Minutes { get; }

    private TimeframeUnit(string name, int minutes)
    {
        Name = name;
        Minutes = minutes;
    }

    public static readonly TimeframeUnit M1 = new("1m", 1);
    public static readonly TimeframeUnit M5 = new("5m", 5);
    public static readonly TimeframeUnit H1 = new("1h", 60);
    public static readonly TimeframeUnit H4 = new("4h", 240);
    public static readonly TimeframeUnit D1 = new("1d", 1440);

    private static readonly HashSet<string> _names = new(StringComparer.OrdinalIgnoreCase)
    {
        M1.Name, M5.Name, H1.Name, H4.Name, D1.Name
    };

    public static bool IsValid(string name) => _names.Contains(name);
}