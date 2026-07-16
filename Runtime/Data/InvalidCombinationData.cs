using System;

namespace EnriRanjan.Interaction
{
    /// <summary>
    /// A known-but-invalid item combination (red herring) with a humor line
    /// to acknowledge the attempt. Matching is order-independent.
    /// </summary>
    public sealed class InvalidCombinationData
    {
        public ItemId ItemA { get; }
        public ItemId ItemB { get; }

        /// <summary>Id of the humor line to play; playback belongs to upper layers.</summary>
        public string HumorLineId { get; }

        public InvalidCombinationData(ItemId itemA, ItemId itemB, string humorLineId)
        {
            if (itemA.IsNone || itemB.IsNone)
            {
                throw new ArgumentException("An invalid combination requires two non-empty item ids.");
            }

            if (string.IsNullOrEmpty(humorLineId))
            {
                throw new ArgumentException("An invalid combination requires a humor line id.", nameof(humorLineId));
            }

            ItemA = itemA;
            ItemB = itemB;
            HumorLineId = humorLineId;
        }

        public bool Matches(ItemId a, ItemId b)
        {
            return (ItemA == a && ItemB == b) || (ItemA == b && ItemB == a);
        }
    }
}
