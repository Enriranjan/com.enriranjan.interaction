using System;

namespace EnriRanjan.Interaction
{
    /// <summary>
    /// Immutable identifier of an item. Wraps a non-empty string; the default
    /// value represents "no item" (<see cref="None"/>).
    /// </summary>
    public readonly struct ItemId : IEquatable<ItemId>
    {
        /// <summary>The "no item" value, equal to <c>default(ItemId)</c>.</summary>
        public static ItemId None => default;

        public string Value { get; }

        public bool IsNone => string.IsNullOrEmpty(Value);

        public ItemId(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("An ItemId value must be a non-empty string.", nameof(value));
            }

            Value = value;
        }

        public bool Equals(ItemId other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is ItemId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value != null ? StringComparer.Ordinal.GetHashCode(Value) : 0;
        }

        public static bool operator ==(ItemId left, ItemId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemId left, ItemId right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return IsNone ? "<none>" : Value;
        }
    }
}
