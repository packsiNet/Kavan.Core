using Ardalis.SmartEnum;

namespace ApplicationLayer.Common.Enums;

public class CourseLevel : SmartEnum<CourseLevel, byte>
{
    public static CourseLevel Beginner = new(nameof(Beginner).ToLower(), 1);
    public static CourseLevel Intermediate = new(nameof(Intermediate).ToLower(), 2);
    public static CourseLevel Advanced = new(nameof(Advanced).ToLower(), 3);
    public static CourseLevel Professional = new(nameof(Professional).ToLower(), 4);

    public CourseLevel(string value, byte id) : base(value, id)
    {
    }
}