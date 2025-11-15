using Ardalis.SmartEnum;

namespace ApplicationLayer.Common.Enums;

public class MediaFileType : SmartEnum<MediaFileType, byte>
{
    public static MediaFileType Video = new(nameof(Video).ToLower(), 1);
    public static MediaFileType Document = new(nameof(Document).ToLower(), 2);
    public static MediaFileType Attachment = new(nameof(Attachment).ToLower(), 3);

    public MediaFileType(string value, byte id) : base(value, id)
    {
    }
}