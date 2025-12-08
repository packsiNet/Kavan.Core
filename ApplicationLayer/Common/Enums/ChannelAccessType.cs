namespace ApplicationLayer.Common.Enums;

public sealed class ChannelAccessType
{
    public string Name { get; }
    public int Value { get; }

    private ChannelAccessType(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public static readonly ChannelAccessType Free = new(nameof(Free), 1);
    public static readonly ChannelAccessType VIP = new(nameof(VIP), 2);

    private static readonly Dictionary<string, ChannelAccessType> _byName = new(StringComparer.OrdinalIgnoreCase)
    {
        { nameof(Free), Free },
        { nameof(VIP), VIP }
    };

    public static bool IsValid(string name) => _byName.ContainsKey(name);

    private static readonly Dictionary<int, ChannelAccessType> _byValue = new()
    {
        { Free.Value, Free },
        { VIP.Value, VIP }
    };

    public static bool IsValid(int value) => _byValue.ContainsKey(value);
    public static ChannelAccessType FromValue(int value) => _byValue[value];
    public static bool TryFromValue(int value, out ChannelAccessType acc) => _byValue.TryGetValue(value, out acc);
}
