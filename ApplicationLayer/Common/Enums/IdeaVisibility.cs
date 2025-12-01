namespace ApplicationLayer.Common.Enums;

public sealed class IdeaVisibility
{
    public string Name { get; }
    public int Value { get; }

    private IdeaVisibility(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public static readonly IdeaVisibility Public = new(nameof(Public), 1);
    public static readonly IdeaVisibility Private = new(nameof(Private), 2);

    private static readonly Dictionary<string, IdeaVisibility> _byName = new(StringComparer.OrdinalIgnoreCase)
    {
        { nameof(Public), Public },
        { nameof(Private), Private }
    };

    public static bool IsValid(string name) => _byName.ContainsKey(name);

    private static readonly Dictionary<int, IdeaVisibility> _byValue = new()
    {
        { Public.Value, Public },
        { Private.Value, Private }
    };

    public static bool IsValid(int value) => _byValue.ContainsKey(value);

    public static IdeaVisibility FromValue(int value) => _byValue[value];

    public static bool TryFromValue(int value, out IdeaVisibility vis) => _byValue.TryGetValue(value, out vis);
}