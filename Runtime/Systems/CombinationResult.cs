namespace EnriRanjan.Interaction
{
    public enum CombinationOutcome
    {
        /// <summary>The pair matches no recipe and no known-invalid combination.</summary>
        Unknown = 0,
        /// <summary>The pair matches a recipe and produces an item.</summary>
        Valid,
        /// <summary>The pair is a known red herring with a humor line.</summary>
        KnownInvalid
    }

    /// <summary>
    /// Discriminated result of <see cref="CombinationSystem.TryCombine"/>.
    /// Only the fields relevant to <see cref="Outcome"/> are populated.
    /// </summary>
    public readonly struct CombinationResult
    {
        public CombinationOutcome Outcome { get; }

        /// <summary>Item produced by the combination; <see cref="ItemId.None"/> unless Valid.</summary>
        public ItemId ResultItem { get; }

        /// <summary>Optional feedback line id of a valid combination; null when absent.</summary>
        public string FeedbackId { get; }

        /// <summary>Humor line id of a known-invalid combination; null otherwise.</summary>
        public string HumorLineId { get; }

        private CombinationResult(CombinationOutcome outcome, ItemId resultItem, string feedbackId, string humorLineId)
        {
            Outcome = outcome;
            ResultItem = resultItem;
            FeedbackId = feedbackId;
            HumorLineId = humorLineId;
        }

        public static CombinationResult Valid(ItemId resultItem, string feedbackId = null)
        {
            return new CombinationResult(CombinationOutcome.Valid, resultItem, feedbackId, null);
        }

        public static CombinationResult KnownInvalid(string humorLineId)
        {
            return new CombinationResult(CombinationOutcome.KnownInvalid, ItemId.None, null, humorLineId);
        }

        public static CombinationResult Unknown()
        {
            return default;
        }
    }
}
