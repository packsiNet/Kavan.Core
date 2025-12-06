namespace ApplicationLayer.Common.Enums;

public sealed class WebsiteType
{
    public string Name { get; }
    public int Value { get; }

    private WebsiteType(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public static readonly WebsiteType Main = new(nameof(Main), 1);
    public static readonly WebsiteType Blog = new(nameof(Blog), 2);
    public static readonly WebsiteType Support = new(nameof(Support), 3);
    public static readonly WebsiteType Docs = new(nameof(Docs), 4);
    public static readonly WebsiteType Official = new(nameof(Official), 5);

    private static readonly HashSet<string> _names = new(StringComparer.OrdinalIgnoreCase)
    { Main.Name, Blog.Name, Support.Name, Docs.Name, Official.Name };

    public static bool IsValid(string name) => string.IsNullOrWhiteSpace(name) || _names.Contains(name);
}
