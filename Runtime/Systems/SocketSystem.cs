using System;
using System.Collections.Generic;

namespace EnriRanjan.Interaction
{
    public enum SocketRejectionReason
    {
        None = 0,
        UnknownSocket,
        InvalidSlotIndex,
        SlotOccupied,
        ItemNotAccepted
    }

    /// <summary>
    /// Tracks socket instances built from <see cref="SocketDefinition"/>s and
    /// the items placed into their slots. Rejections carry no penalty:
    /// <see cref="TryPlace(string, ItemId, int?, out SocketRejectionReason)"/>
    /// returns false with a reason and raises <see cref="ItemRejected"/>.
    /// </summary>
    public sealed class SocketSystem
    {
        private sealed class SocketInstance
        {
            public SocketDefinition Definition;
            public ItemId[] Slots; // default(ItemId) == empty slot
        }

        private readonly Dictionary<string, SocketInstance> _sockets =
            new Dictionary<string, SocketInstance>();

        /// <summary>Raised when an item is accepted: (socketInstanceId, itemId, slotIndex).</summary>
        public event Action<string, ItemId, int> ItemSocketed;

        /// <summary>Raised when a placement is rejected: (socketInstanceId, itemId, reason).</summary>
        public event Action<string, ItemId, SocketRejectionReason> ItemRejected;

        /// <summary>Raised when every slot of a socket holds an accepted item: (socketInstanceId).</summary>
        public event Action<string> SocketCompleted;

        public void RegisterSocket(string socketInstanceId, SocketDefinition definition)
        {
            if (string.IsNullOrEmpty(socketInstanceId))
            {
                throw new ArgumentException("A socket instance requires a non-empty id.", nameof(socketInstanceId));
            }

            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (_sockets.ContainsKey(socketInstanceId))
            {
                throw new InvalidOperationException(
                    $"A socket instance with id '{socketInstanceId}' is already registered.");
            }

            _sockets.Add(socketInstanceId, new SocketInstance
            {
                Definition = definition,
                Slots = new ItemId[definition.Slots.Count]
            });
        }

        public bool TryPlace(string socketInstanceId, ItemId itemId, out SocketRejectionReason reason)
        {
            return TryPlace(socketInstanceId, itemId, null, out reason);
        }

        /// <summary>
        /// Attempts to place <paramref name="itemId"/> into the socket. When
        /// <paramref name="slotIndex"/> is null, a single-slot socket targets
        /// its only slot and a multi-slot socket targets the first empty slot
        /// that accepts the item.
        /// </summary>
        public bool TryPlace(string socketInstanceId, ItemId itemId, int? slotIndex, out SocketRejectionReason reason)
        {
            if (!_sockets.TryGetValue(socketInstanceId, out SocketInstance socket))
            {
                return Reject(socketInstanceId, itemId, SocketRejectionReason.UnknownSocket, out reason);
            }

            int targetSlot;

            if (slotIndex.HasValue)
            {
                targetSlot = slotIndex.Value;

                if (targetSlot < 0 || targetSlot >= socket.Slots.Length)
                {
                    return Reject(socketInstanceId, itemId, SocketRejectionReason.InvalidSlotIndex, out reason);
                }

                if (!socket.Slots[targetSlot].IsNone)
                {
                    return Reject(socketInstanceId, itemId, SocketRejectionReason.SlotOccupied, out reason);
                }

                if (!socket.Definition.Slots[targetSlot].Accepts(itemId))
                {
                    return Reject(socketInstanceId, itemId, SocketRejectionReason.ItemNotAccepted, out reason);
                }
            }
            else
            {
                targetSlot = -1;
                bool anySlotAccepts = false;

                for (int i = 0; i < socket.Slots.Length; i++)
                {
                    if (!socket.Definition.Slots[i].Accepts(itemId))
                    {
                        continue;
                    }

                    anySlotAccepts = true;

                    if (socket.Slots[i].IsNone)
                    {
                        targetSlot = i;
                        break;
                    }
                }

                if (targetSlot < 0)
                {
                    SocketRejectionReason rejection = anySlotAccepts
                        ? SocketRejectionReason.SlotOccupied
                        : SocketRejectionReason.ItemNotAccepted;
                    return Reject(socketInstanceId, itemId, rejection, out reason);
                }
            }

            socket.Slots[targetSlot] = itemId;
            reason = SocketRejectionReason.None;
            ItemSocketed?.Invoke(socketInstanceId, itemId, targetSlot);

            if (IsComplete(socket))
            {
                SocketCompleted?.Invoke(socketInstanceId);
            }

            return true;
        }

        /// <summary>
        /// Removes the item at <paramref name="slotIndex"/>. Returns false when
        /// the socket is unknown, the index is out of range or the slot is empty.
        /// </summary>
        public bool TryRemove(string socketInstanceId, out ItemId removedItem, int slotIndex = 0)
        {
            removedItem = ItemId.None;

            if (!_sockets.TryGetValue(socketInstanceId, out SocketInstance socket))
            {
                return false;
            }

            if (slotIndex < 0 || slotIndex >= socket.Slots.Length || socket.Slots[slotIndex].IsNone)
            {
                return false;
            }

            removedItem = socket.Slots[slotIndex];
            socket.Slots[slotIndex] = ItemId.None;
            return true;
        }

        /// <summary>Item currently in the slot, or <see cref="ItemId.None"/> when empty.</summary>
        public ItemId GetItemAt(string socketInstanceId, int slotIndex)
        {
            SocketInstance socket = GetSocket(socketInstanceId);

            if (slotIndex < 0 || slotIndex >= socket.Slots.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(slotIndex));
            }

            return socket.Slots[slotIndex];
        }

        public bool IsComplete(string socketInstanceId)
        {
            return IsComplete(GetSocket(socketInstanceId));
        }

        private SocketInstance GetSocket(string socketInstanceId)
        {
            if (!_sockets.TryGetValue(socketInstanceId, out SocketInstance socket))
            {
                throw new InvalidOperationException(
                    $"No socket instance with id '{socketInstanceId}' is registered.");
            }

            return socket;
        }

        private static bool IsComplete(SocketInstance socket)
        {
            for (int i = 0; i < socket.Slots.Length; i++)
            {
                if (socket.Slots[i].IsNone)
                {
                    return false;
                }
            }

            return true;
        }

        private bool Reject(string socketInstanceId, ItemId itemId, SocketRejectionReason rejection, out SocketRejectionReason reason)
        {
            reason = rejection;
            ItemRejected?.Invoke(socketInstanceId, itemId, rejection);
            return false;
        }
    }
}
