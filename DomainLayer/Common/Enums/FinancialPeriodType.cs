namespace DomainLayer.Common.Enums;

public sealed class FinancialPeriodType
{
    public string Name { get; }
    public int Value { get; }

    private FinancialPeriodType(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public static readonly FinancialPeriodType Weekly = new(nameof(Weekly), 1);
    public static readonly FinancialPeriodType BiWeekly = new(nameof(BiWeekly), 2);
    public static readonly FinancialPeriodType Monthly = new(nameof(Monthly), 3);
    public static readonly FinancialPeriodType Custom = new(nameof(Custom), 4);

    private static readonly Dictionary<string, FinancialPeriodType> _byName = new(StringComparer.OrdinalIgnoreCase)
    {
        { nameof(Weekly), Weekly },
        { nameof(BiWeekly), BiWeekly },
        { nameof(Monthly), Monthly },
        { nameof(Custom), Custom }
    };

    public static bool IsValid(string name) => _byName.ContainsKey(name);

    private static readonly Dictionary<int, FinancialPeriodType> _byValue = new()
    {
        { Weekly.Value, Weekly },
        { BiWeekly.Value, BiWeekly },
        { Monthly.Value, Monthly },
        { Custom.Value, Custom }
    };

    public static bool IsValid(int value) => _byValue.ContainsKey(value);
    public static FinancialPeriodType FromValue(int value) => _byValue[value];
    public static bool TryFromValue(int value, out FinancialPeriodType t) => _byValue.TryGetValue(value, out t);

    public static implicit operator int(FinancialPeriodType t) => t.Value;
}
