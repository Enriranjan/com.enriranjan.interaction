using NUnit.Framework;

namespace EnriRanjan.Interaction.Tests
{
    public class CombinationSystemTests
    {
        private static readonly ItemId Rope = new ItemId("rope");
        private static readonly ItemId Hook = new ItemId("hook");
        private static readonly ItemId GrapplingHook = new ItemId("grappling-hook");
        private static readonly ItemId Banana = new ItemId("banana");
        private static readonly ItemId Candle = new ItemId("candle");

        private static CombinationSystem CreateSystem()
        {
            return new CombinationSystem(
                new[] { new RecipeData(Rope, Hook, GrapplingHook, "vo_grappling_hook_created") },
                new[] { new InvalidCombinationData(Rope, Banana, "vo_rope_banana_joke") });
        }

        [Test]
        public void TryCombine_ValidRecipe_ReturnsResultAndFeedback()
        {
            var system = CreateSystem();

            CombinationResult result = system.TryCombine(Rope, Hook);

            Assert.AreEqual(CombinationOutcome.Valid, result.Outcome);
            Assert.AreEqual(GrapplingHook, result.ResultItem);
            Assert.AreEqual("vo_grappling_hook_created", result.FeedbackId);
        }

        [Test]
        public void TryCombine_ValidRecipe_IsOrderIndependent()
        {
            var system = CreateSystem();

            CombinationResult result = system.TryCombine(Hook, Rope);

            Assert.AreEqual(CombinationOutcome.Valid, result.Outcome);
            Assert.AreEqual(GrapplingHook, result.ResultItem);
        }

        [Test]
        public void TryCombine_KnownInvalid_ReturnsHumorLine()
        {
            var system = CreateSystem();

            CombinationResult result = system.TryCombine(Banana, Rope);

            Assert.AreEqual(CombinationOutcome.KnownInvalid, result.Outcome);
            Assert.AreEqual("vo_rope_banana_joke", result.HumorLineId);
            Assert.IsTrue(result.ResultItem.IsNone);
        }

        [Test]
        public void TryCombine_UnknownCombination_ReturnsUnknown()
        {
            var system = CreateSystem();

            CombinationResult result = system.TryCombine(Rope, Candle);

            Assert.AreEqual(CombinationOutcome.Unknown, result.Outcome);
            Assert.IsTrue(result.ResultItem.IsNone);
            Assert.IsNull(result.FeedbackId);
            Assert.IsNull(result.HumorLineId);
        }
    }
}
