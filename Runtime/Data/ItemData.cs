using System;
using System.Collections.Generic;

namespace EnriRanjan.Interaction
{
    /// <summary>
    /// Engine-agnostic definition of an interactable item. Pure data: no
    /// behaviour, no references to game content beyond ids.
    /// </summary>
    public sealed class ItemData
    {
        public ItemId Id { get; }
        public string Name { get; }
        public string Description { get; }
        public ItemCapabilities Capabilities { get; }

        /// <summary>
        /// Id of the <see cref="StateDefinition"/> driving this item, or null
        /// when the item is not <see cref="ItemCapabilities.Stateful"/>.
        /// </summary>
        public string StateDefinitionId { get; }

        /// <summary>
        /// Id of the narrative entry this item triggers, or null when the item
        /// is not a <see cref="ItemCapabilities.NarrativeTrigger"/>.
        /// </summary>
        public string NarrativeId { get; }

        /// <summary>
        /// Items initially held when the item is a
        /// <see cref="ItemCapabilities.Container"/>; empty otherwise.
        /// </summary>
        public IReadOnlyList<ItemId> InitialContents { get; }

        public ItemData(
            ItemId id,
            string name,
            string description,
            ItemCapabilities capabilities,
            string stateDefinitionId = null,
            string narrativeId = null,
            IEnumerable<ItemId> initialContents = null)
        {
            if (id.IsNone)
            {
                throw new ArgumentException("ItemData requires a non-empty id.", nameof(id));
            }

            Id = id;
            Name = name;
            Description = description;
            Capabilities = capabilities;
            StateDefinitionId = stateDefinitionId;
            NarrativeId = narrativeId;
            InitialContents = initialContents != null
                ? (IReadOnlyList<ItemId>)new List<ItemId>(initialContents)
                : Array.Empty<ItemId>();
        }

        public bool HasCapability(ItemCapabilities capability)
        {
            return (Capabilities & capability) == capability;
        }
    }
}
