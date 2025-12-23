namespace DomainLayer.Common.Enums;

public sealed class ExitReason
{
    public string Name { get; }
    public int Value { get; }

    private ExitReason(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public static readonly ExitReason TakeProfit = new(nameof(TakeProfit), 1);
    public static readonly ExitReason StopLoss = new(nameof(StopLoss), 2);
    public static readonly ExitReason ManualExit = new(nameof(ManualExit), 3); // Rename Manual to ManualExit to match prompt if needed, or alias it. Prompt used "ManualExit".
    public static readonly ExitReason TimeExpiry = new(nameof(TimeExpiry), 4);

    private static readonly Dictionary<string, ExitReason> _byName = new(StringComparer.OrdinalIgnoreCase)
    {
        { nameof(TakeProfit), TakeProfit },
        { nameof(StopLoss), StopLoss },
        { nameof(ManualExit), ManualExit },
        { nameof(TimeExpiry), TimeExpiry }
    };

    public static bool IsValid(string name) => _byName.ContainsKey(name);

    private static readonly Dictionary<int, ExitReason> _byValue = new()
    {
        { TakeProfit.Value, TakeProfit },
        { StopLoss.Value, StopLoss },
        { ManualExit.Value, ManualExit },
        { TimeExpiry.Value, TimeExpiry }
    };

    public static bool IsValid(int value) => _byValue.ContainsKey(value);
    public static ExitReason FromValue(int value) => _byValue[value];
    public static bool TryFromValue(int value, out ExitReason t) => _byValue.TryGetValue(value, out t);

    public static implicit operator int(ExitReason t) => t.Value;
}
