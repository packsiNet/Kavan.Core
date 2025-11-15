namespace ApplicationLayer.Common.Enums
{
    // Lightweight SmartEnum-style implementation without external packages
    public sealed class SignalStrength : IEquatable<SignalStrength>
    {
        public string Name { get; }

        public int Value { get; }

        private SignalStrength(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public static readonly SignalStrength Weak = new("Weak", 1);
        public static readonly SignalStrength Medium = new("Medium", 2);
        public static readonly SignalStrength Strong = new("Strong", 3);

        public static SignalStrength FromScore(double score, double weakMax, double mediumMax)
        {
            if (score < weakMax) return Weak;
            if (score < mediumMax) return Medium;
            return Strong;
        }

        public override string ToString() => Name;

        public bool Equals(SignalStrength other)
            => other is not null && other.Value == Value;

        public override bool Equals(object obj)
            => obj is SignalStrength other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();
    }
}