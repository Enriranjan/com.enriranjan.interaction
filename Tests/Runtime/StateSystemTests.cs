using NUnit.Framework;

namespace EnriRanjan.Interaction.Tests
{
    public class StateSystemTests
    {
        private static readonly ItemId MusicBox = new ItemId("music-box");

        private static StateSystem CreateSystemWithRegisteredItem()
        {
            var definition = new StateDefinition(
                "music-box-states",
                new[] { "closed", "open", "playing" },
                "closed",
                new[]
                {
                    new StateTransition("closed", "open-lid", "open"),
                    new StateTransition("open", "wind-up", "playing")
                });

            var system = new StateSystem(new[] { definition });
            system.RegisterItem(MusicBox, "music-box-states");
            return system;
        }

        [Test]
        public void RegisterItem_StartsAtInitialState()
        {
            var system = CreateSystemWithRegisteredItem();

            Assert.AreEqual("closed", system.GetState(MusicBox));
        }

        [Test]
        public void TryTransition_LegalTrigger_ChangesStateAndRaisesEvent()
        {
            var system = CreateSystemWithRegisteredItem();
            ItemId eventItem = ItemId.None;
            string eventFrom = null;
            string eventTo = null;
            system.StateChanged += (item, from, to) =>
            {
                eventItem = item;
                eventFrom = from;
                eventTo = to;
            };

            bool transitioned = system.TryTransition(MusicBox, "open-lid");

            Assert.IsTrue(transitioned);
            Assert.AreEqual("open", system.GetState(MusicBox));
            Assert.AreEqual(MusicBox, eventItem);
            Assert.AreEqual("closed", eventFrom);
            Assert.AreEqual("open", eventTo);
        }

        [Test]
        public void TryTransition_IllegalTrigger_ReturnsFalseAndKeepsState()
        {
            var system = CreateSystemWithRegisteredItem();
            bool eventRaised = false;
            system.StateChanged += (item, from, to) => eventRaised = true;

            // "wind-up" is only legal from "open"; the item is still "closed".
            bool transitioned = system.TryTransition(MusicBox, "wind-up");

            Assert.IsFalse(transitioned);
            Assert.AreEqual("closed", system.GetState(MusicBox));
            Assert.IsFalse(eventRaised);
        }

        [Test]
        public void TryTransition_UnregisteredItem_ReturnsFalse()
        {
            var system = CreateSystemWithRegisteredItem();

            Assert.IsFalse(system.TryTransition(new ItemId("unknown-item"), "open-lid"));
        }
    }
}
