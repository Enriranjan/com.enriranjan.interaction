using System;
using System.Collections.Generic;

namespace EnriRanjan.Interaction
{
    /// <summary>
    /// Tracks the contents of revealable containers. A container must be
    /// revealed before items can be taken from it; revealing is idempotent
    /// and <see cref="ContainerRevealed"/> fires only on the first reveal.
    /// </summary>
    public sealed class ContainerSystem
    {
        private sealed class ContainerInstance
        {
            public List<ItemId> Contents;
            public bool Revealed;
        }

        private readonly Dictionary<ItemId, ContainerInstance> _containers =
            new Dictionary<ItemId, ContainerInstance>();

        /// <summary>Raised on the first reveal: (containerId, revealed items).</summary>
        public event Action<ItemId, IReadOnlyList<ItemId>> ContainerRevealed;

        public void RegisterContainer(ItemId containerId, IEnumerable<ItemId> initialContents)
        {
            if (containerId.IsNone)
            {
                throw new ArgumentException("Cannot register a container for ItemId.None.", nameof(containerId));
            }

            if (_containers.ContainsKey(containerId))
            {
                throw new InvalidOperationException($"Container '{containerId}' is already registered.");
            }

            _containers.Add(containerId, new ContainerInstance
            {
                Contents = initialContents != null ? new List<ItemId>(initialContents) : new List<ItemId>(),
                Revealed = false
            });
        }

        /// <summary>Marks the container as revealed and returns its current contents.</summary>
        public IReadOnlyList<ItemId> Reveal(ItemId containerId)
        {
            ContainerInstance container = GetContainer(containerId);
            ItemId[] contents = container.Contents.ToArray();

            if (!container.Revealed)
            {
                container.Revealed = true;
                ContainerRevealed?.Invoke(containerId, contents);
            }

            return contents;
        }

        /// <summary>
        /// Removes <paramref name="itemId"/> from the container. Returns false
        /// when the container is unknown, not yet revealed, or does not hold the item.
        /// </summary>
        public bool TakeFromContainer(ItemId containerId, ItemId itemId)
        {
            if (!_containers.TryGetValue(containerId, out ContainerInstance container))
            {
                return false;
            }

            if (!container.Revealed)
            {
                return false;
            }

            return container.Contents.Remove(itemId);
        }

        public bool IsRevealed(ItemId containerId)
        {
            return GetContainer(containerId).Revealed;
        }

        public IReadOnlyList<ItemId> GetContents(ItemId containerId)
        {
            return GetContainer(containerId).Contents.ToArray();
        }

        private ContainerInstance GetContainer(ItemId containerId)
        {
            if (!_containers.TryGetValue(containerId, out ContainerInstance container))
            {
                throw new InvalidOperationException($"No container with id '{containerId}' is registered.");
            }

            return container;
        }
    }
}
