using System;
using System.Collections.Generic;

namespace EnriRanjan.Interaction
{
    /// <summary>
    /// Holds one discrete state machine instance per registered item, driven
    /// by the shared <see cref="StateDefinition"/>s supplied at construction.
    /// </summary>
    public sealed class StateSystem
    {
        private sealed class StateInstance
        {
            public StateDefinition Definition;
            public string CurrentState;
        }

        private readonly Dictionary<string, StateDefinition> _definitions =
            new Dictionary<string, StateDefinition>();
        private readonly Dictionary<ItemId, StateInstance> _instances =
            new Dictionary<ItemId, StateInstance>();

        /// <summary>Raised after a successful transition: (itemId, fromState, toState).</summary>
        public event Action<ItemId, string, string> StateChanged;

        public StateSystem(IEnumerable<StateDefinition> definitions)
        {
            if (definitions == null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }

            foreach (StateDefinition definition in definitions)
            {
                _definitions.Add(definition.DefinitionId, definition);
            }
        }

        /// <summary>Creates a state instance for the item at the definition's initial state.</summary>
        public void RegisterItem(ItemId itemId, string stateDefinitionId)
        {
            if (itemId.IsNone)
            {
                throw new ArgumentException("Cannot register a state instance for ItemId.None.", nameof(itemId));
            }

            if (!_definitions.TryGetValue(stateDefinitionId ?? string.Empty, out StateDefinition definition))
            {
                throw new InvalidOperationException(
                    $"No state definition with id '{stateDefinitionId}' is known.");
            }

            if (_instances.ContainsKey(itemId))
            {
                throw new InvalidOperationException(
                    $"Item '{itemId}' already has a state instance.");
            }

            _instances.Add(itemId, new StateInstance
            {
                Definition = definition,
                CurrentState = definition.InitialState
            });
        }

        /// <summary>
        /// Applies <paramref name="trigger"/> to the item's state machine.
        /// Returns false (and changes nothing) when the item has no state
        /// instance or no transition matches the current state and trigger.
        /// </summary>
        public bool TryTransition(ItemId itemId, string trigger)
        {
            if (!_instances.TryGetValue(itemId, out StateInstance instance))
            {
                return false;
            }

            if (!instance.Definition.TryGetTransition(instance.CurrentState, trigger, out string toState))
            {
                return false;
            }

            string fromState = instance.CurrentState;
            instance.CurrentState = toState;
            StateChanged?.Invoke(itemId, fromState, toState);
            return true;
        }

        public string GetState(ItemId itemId)
        {
            if (!TryGetState(itemId, out string state))
            {
                throw new InvalidOperationException($"Item '{itemId}' has no state instance.");
            }

            return state;
        }

        public bool TryGetState(ItemId itemId, out string state)
        {
            if (_instances.TryGetValue(itemId, out StateInstance instance))
            {
                state = instance.CurrentState;
                return true;
            }

            state = null;
            return false;
        }
    }
}
