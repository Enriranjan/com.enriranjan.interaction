using System;
using System.Collections.Generic;

namespace EnriRanjan.Interaction
{
    /// <summary>
    /// One receiving slot of a socket: the set of item ids it accepts.
    /// </summary>
    public sealed class SocketSlotDefinition
    {
        private readonly HashSet<ItemId> _acceptedItems;

        public IReadOnlyCollection<ItemId> AcceptedItems => _acceptedItems;

        public SocketSlotDefinition(IEnumerable<ItemId> acceptedItems)
        {
            if (acceptedItems == null)
            {
                throw new ArgumentNullException(nameof(acceptedItems));
            }

            _acceptedItems = new HashSet<ItemId>(acceptedItems);

            if (_acceptedItems.Count == 0)
            {
                throw new ArgumentException("A socket slot must accept at least one item.", nameof(acceptedItems));
            }
        }

        public SocketSlotDefinition(params ItemId[] acceptedItems)
            : this((IEnumerable<ItemId>)acceptedItems)
        {
        }

        public bool Accepts(ItemId itemId)
        {
            return _acceptedItems.Contains(itemId);
        }
    }

    /// <summary>
    /// Definition of a scene socket that receives items. A single-slot socket
    /// has one slot; a multi-slot socket has an ordered list of slots, each of
    /// which may expect specific items (e.g. a lectern with 5 plates).
    /// </summary>
    public sealed class SocketDefinition
    {
        public string SocketId { get; }

        /// <summary>Ordered slots; index is the slot index used by the socket system.</summary>
        public IReadOnlyList<SocketSlotDefinition> Slots { get; }

        public bool IsSingleSlot => Slots.Count == 1;

        public SocketDefinition(string socketId, IEnumerable<SocketSlotDefinition> slots)
        {
            if (string.IsNullOrEmpty(socketId))
            {
                throw new ArgumentException("A socket definition requires a non-empty id.", nameof(socketId));
            }

            if (slots == null)
            {
                throw new ArgumentNullException(nameof(slots));
            }

            var slotList = new List<SocketSlotDefinition>(slots);

            if (slotList.Count == 0)
            {
                throw new ArgumentException("A socket definition requires at least one slot.", nameof(slots));
            }

            SocketId = socketId;
            Slots = slotList;
        }

        /// <summary>Convenience for the single-slot case.</summary>
        public SocketDefinition(string socketId, params ItemId[] acceptedItems)
            : this(socketId, new[] { new SocketSlotDefinition(acceptedItems) })
        {
        }
    }
}
