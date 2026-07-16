using System.Collections.Generic;
using NUnit.Framework;

namespace EnriRanjan.Interaction.Tests
{
    public class SocketSystemTests
    {
        private static readonly ItemId BrassKey = new ItemId("brass-key");
        private static readonly ItemId RustyKey = new ItemId("rusty-key");
        private static readonly ItemId PlateSun = new ItemId("plate-sun");
        private static readonly ItemId PlateMoon = new ItemId("plate-moon");
        private static readonly ItemId PlateStar = new ItemId("plate-star");

        private const string Keyhole = "keyhole-instance";
        private const string Lectern = "lectern-instance";

        private static SocketSystem CreateSystemWithKeyhole()
        {
            var system = new SocketSystem();
            system.RegisterSocket(Keyhole, new SocketDefinition("keyhole", BrassKey));
            return system;
        }

        private static SocketSystem CreateSystemWithLectern()
        {
            var system = new SocketSystem();
            system.RegisterSocket(Lectern, new SocketDefinition("lectern", new[]
            {
                new SocketSlotDefinition(PlateSun),
                new SocketSlotDefinition(PlateMoon),
                new SocketSlotDefinition(PlateStar)
            }));
            return system;
        }

        [Test]
        public void TryPlace_AcceptedItem_SucceedsAndRaisesItemSocketed()
        {
            var system = CreateSystemWithKeyhole();
            var socketedEvents = new List<(string socketId, ItemId item, int slot)>();
            system.ItemSocketed += (socketId, item, slot) => socketedEvents.Add((socketId, item, slot));

            bool placed = system.TryPlace(Keyhole, BrassKey, out SocketRejectionReason reason);

            Assert.IsTrue(placed);
            Assert.AreEqual(SocketRejectionReason.None, reason);
            Assert.AreEqual(1, socketedEvents.Count);
            Assert.AreEqual((Keyhole, BrassKey, 0), socketedEvents[0]);
            Assert.AreEqual(BrassKey, system.GetItemAt(Keyhole, 0));
        }

        [Test]
        public void TryPlace_NotAcceptedItem_FailsAndRaisesItemRejected()
        {
            var system = CreateSystemWithKeyhole();
            var rejectedEvents = new List<(string socketId, ItemId item, SocketRejectionReason reason)>();
            system.ItemRejected += (socketId, item, reason) => rejectedEvents.Add((socketId, item, reason));

            bool placed = system.TryPlace(Keyhole, RustyKey, out SocketRejectionReason rejection);

            Assert.IsFalse(placed);
            Assert.AreEqual(SocketRejectionReason.ItemNotAccepted, rejection);
            Assert.AreEqual(1, rejectedEvents.Count);
            Assert.AreEqual((Keyhole, RustyKey, SocketRejectionReason.ItemNotAccepted), rejectedEvents[0]);
            Assert.IsTrue(system.GetItemAt(Keyhole, 0).IsNone);
        }

        [Test]
        public void TryPlace_OccupiedSingleSlot_FailsWithSlotOccupied()
        {
            var system = CreateSystemWithKeyhole();
            system.TryPlace(Keyhole, BrassKey, out _);

            bool placed = system.TryPlace(Keyhole, BrassKey, out SocketRejectionReason rejection);

            Assert.IsFalse(placed);
            Assert.AreEqual(SocketRejectionReason.SlotOccupied, rejection);
        }

        [Test]
        public void TryPlace_OrderedMultiSlot_RaisesSocketCompletedWhenAllSlotsFilled()
        {
            var system = CreateSystemWithLectern();
            var completedSockets = new List<string>();
            system.SocketCompleted += socketId => completedSockets.Add(socketId);

            Assert.IsTrue(system.TryPlace(Lectern, PlateSun, 0, out _));
            Assert.IsTrue(system.TryPlace(Lectern, PlateMoon, 1, out _));
            Assert.IsEmpty(completedSockets, "Socket must not complete while a slot is still empty.");

            Assert.IsTrue(system.TryPlace(Lectern, PlateStar, 2, out _));

            Assert.AreEqual(new[] { Lectern }, completedSockets);
            Assert.IsTrue(system.IsComplete(Lectern));
        }

        [Test]
        public void TryPlace_OrderedMultiSlot_RejectsItemInWrongSlot()
        {
            var system = CreateSystemWithLectern();

            bool placed = system.TryPlace(Lectern, PlateMoon, 0, out SocketRejectionReason rejection);

            Assert.IsFalse(placed);
            Assert.AreEqual(SocketRejectionReason.ItemNotAccepted, rejection);
        }

        [Test]
        public void TryRemove_OccupiedSlot_ReturnsItemAndFreesSlot()
        {
            var system = CreateSystemWithKeyhole();
            system.TryPlace(Keyhole, BrassKey, out _);

            bool removed = system.TryRemove(Keyhole, out ItemId removedItem);

            Assert.IsTrue(removed);
            Assert.AreEqual(BrassKey, removedItem);
            Assert.IsTrue(system.GetItemAt(Keyhole, 0).IsNone);
        }
    }
}
