namespace ApplicationLayer.Common.Enums;

public sealed class SocialPlatform
{
    public string Name { get; }
    public int Value { get; }

    private SocialPlatform(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public static readonly SocialPlatform Twitter = new("Twitter", 1);
    public static readonly SocialPlatform Telegram = new("Telegram", 2);
    public static readonly SocialPlatform LinkedIn = new("LinkedIn", 3);
    public static readonly SocialPlatform Instagram = new("Instagram", 4);
    public static readonly SocialPlatform YouTube = new("YouTube", 5);
    public static readonly SocialPlatform GitHub = new("GitHub", 6);

    private static readonly HashSet<string> _names = new(StringComparer.OrdinalIgnoreCase)
    { Twitter.Name, Telegram.Name, LinkedIn.Name, Instagram.Name, YouTube.Name, GitHub.Name };

    public static bool IsValid(string name) => string.IsNullOrWhiteSpace(name) || _names.Contains(name);
}