using System;

namespace EnriRanjan.Interaction
{
    /// <summary>
    /// A valid item combination: A + B produces <see cref="Result"/>.
    /// Matching is order-independent (A+B == B+A).
    /// </summary>
    public sealed class RecipeData
    {
        public ItemId ItemA { get; }
        public ItemId ItemB { get; }
        public ItemId Result { get; }

        /// <summary>
        /// Optional id of an audio/text feedback line. Playback belongs to
        /// upper layers; this is only an id.
        /// </summary>
        public string FeedbackId { get; }

        public RecipeData(ItemId itemA, ItemId itemB, ItemId result, string feedbackId = null)
        {
            if (itemA.IsNone || itemB.IsNone || result.IsNone)
            {
                throw new ArgumentException("A recipe requires two non-empty ingredient ids and a non-empty result id.");
            }

            ItemA = itemA;
            ItemB = itemB;
            Result = result;
            FeedbackId = feedbackId;
        }

        public bool Matches(ItemId a, ItemId b)
        {
            return (ItemA == a && ItemB == b) || (ItemA == b && ItemB == a);
        }
    }
}
