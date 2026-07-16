using System;
using System.Collections.Generic;

namespace EnriRanjan.Interaction
{
    /// <summary>
    /// Resolves item pair combinations against the known recipes and the
    /// known-invalid (red herring) combinations. Matching is order-independent.
    /// </summary>
    public sealed class CombinationSystem
    {
        private readonly List<RecipeData> _recipes;
        private readonly List<InvalidCombinationData> _knownInvalidCombinations;

        public CombinationSystem(
            IEnumerable<RecipeData> recipes,
            IEnumerable<InvalidCombinationData> knownInvalidCombinations)
        {
            if (recipes == null)
            {
                throw new ArgumentNullException(nameof(recipes));
            }

            if (knownInvalidCombinations == null)
            {
                throw new ArgumentNullException(nameof(knownInvalidCombinations));
            }

            _recipes = new List<RecipeData>(recipes);
            _knownInvalidCombinations = new List<InvalidCombinationData>(knownInvalidCombinations);
        }

        public CombinationResult TryCombine(ItemId a, ItemId b)
        {
            for (int i = 0; i < _recipes.Count; i++)
            {
                if (_recipes[i].Matches(a, b))
                {
                    return CombinationResult.Valid(_recipes[i].Result, _recipes[i].FeedbackId);
                }
            }

            for (int i = 0; i < _knownInvalidCombinations.Count; i++)
            {
                if (_knownInvalidCombinations[i].Matches(a, b))
                {
                    return CombinationResult.KnownInvalid(_knownInvalidCombinations[i].HumorLineId);
                }
            }

            return CombinationResult.Unknown();
        }
    }
}
