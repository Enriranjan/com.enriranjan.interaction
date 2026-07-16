using System;
using System.Collections.Generic;

namespace EnriRanjan.Interaction
{
    /// <summary>
    /// Read-only catalog of every <see cref="ItemData"/> the game knows about,
    /// keyed by <see cref="ItemId"/>.
    /// </summary>
    public sealed class ItemCatalogSystem
    {
        private readonly Dictionary<ItemId, ItemData> _items;

        public ItemCatalogSystem(IReadOnlyCollection<ItemData> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            _items = new Dictionary<ItemId, ItemData>(items.Count);
            foreach (ItemData item in items)
            {
                _items.Add(item.Id, item);
            }
        }

        /// <summary>Returns the item; throws when the id is unknown.</summary>
        public ItemData GetItem(ItemId id)
        {
            if (!TryGetItem(id, out ItemData item))
            {
                throw new InvalidOperationException($"No item with id '{id}' exists in the catalog.");
            }

            return item;
        }

        public bool TryGetItem(ItemId id, out ItemData item)
        {
            return _items.TryGetValue(id, out item);
        }

        /// <summary>False when the item is unknown or lacks the capability.</summary>
        public bool HasCapability(ItemId id, ItemCapabilities capability)
        {
            return TryGetItem(id, out ItemData item) && item.HasCapability(capability);
        }
    }
}
