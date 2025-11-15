using Ardalis.SmartEnum;

namespace ApplicationLayer.Common.Enums;

public sealed class ApiDefinitions : SmartEnum<ApiDefinitions, byte>
{
    #region Fields

    public static readonly ApiDefinitions Public = new(1, "کاربران عمومی", nameof(Public));
    public static readonly ApiDefinitions Mobile = new(2, "اپلیکیشن موبایل", nameof(Mobile));
    public static readonly ApiDefinitions Admin = new(3, "پنل مدیریت", nameof(Admin));
    public static readonly ApiDefinitions Trader = new(4, "پنل تریدرها", nameof(Trader));
    public static readonly ApiDefinitions External = new(5, "سرویس‌های خارجی", nameof(External));

    #endregion Fields

    #region Properties

    public string PersianName { get; }

    public string EnglishName { get; }

    #endregion Properties

    #region Constructors

    private ApiDefinitions(byte id, string persianName, string englishName)
        : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }

    #endregion Constructors
}