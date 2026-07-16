using System.Collections.Generic;
using NUnit.Framework;

namespace EnriRanjan.Interaction.Tests
{
    public class ContainerSystemTests
    {
        private static readonly ItemId JewelryBox = new ItemId("jewelry-box");
        private static readonly ItemId Locket = new ItemId("locket");
        private static readonly ItemId Photograph = new ItemId("photograph");

        private static ContainerSystem CreateSystem()
        {
            var system = new ContainerSystem();
            system.RegisterContainer(JewelryBox, new[] { Locket, Photograph });
            return system;
        }

        [Test]
        public void Reveal_ReturnsContentsAndRaisesEventOnce()
        {
            var system = CreateSystem();
            var revealedEvents = new List<(ItemId container, IReadOnlyList<ItemId> items)>();
            system.ContainerRevealed += (container, items) => revealedEvents.Add((container, items));

            IReadOnlyList<ItemId> revealed = system.Reveal(JewelryBox);
            system.Reveal(JewelryBox);

            Assert.AreEqual(new[] { Locket, Photograph }, revealed);
            Assert.AreEqual(1, revealedEvents.Count, "ContainerRevealed must fire only on the first reveal.");
            Assert.AreEqual(JewelryBox, revealedEvents[0].container);
            Assert.IsTrue(system.IsRevealed(JewelryBox));
        }

        [Test]
        public void TakeFromContainer_BeforeReveal_ReturnsFalse()
        {
            var system = CreateSystem();

            Assert.IsFalse(system.TakeFromContainer(JewelryBox, Locket));
            Assert.AreEqual(new[] { Locket, Photograph }, system.GetContents(JewelryBox));
        }

        [Test]
        public void TakeFromContainer_AfterReveal_RemovesItem()
        {
            var system = CreateSystem();
            system.Reveal(JewelryBox);

            Assert.IsTrue(system.TakeFromContainer(JewelryBox, Locket));
            Assert.AreEqual(new[] { Photograph }, system.GetContents(JewelryBox));
            Assert.IsFalse(system.TakeFromContainer(JewelryBox, Locket), "An item can only be taken once.");
        }
    }
}
