using System;
using System.Collections.Generic;

namespace EnriRanjan.Interaction
{
    /// <summary>
    /// One legal transition of a <see cref="StateDefinition"/>:
    /// while in <see cref="FromState"/>, <see cref="Trigger"/> moves to <see cref="ToState"/>.
    /// </summary>
    public sealed class StateTransition
    {
        public string FromState { get; }
        public string Trigger { get; }
        public string ToState { get; }

        public StateTransition(string fromState, string trigger, string toState)
        {
            if (string.IsNullOrEmpty(fromState) || string.IsNullOrEmpty(trigger) || string.IsNullOrEmpty(toState))
            {
                throw new ArgumentException("A state transition requires non-empty from/trigger/to values.");
            }

            FromState = fromState;
            Trigger = trigger;
            ToState = toState;
        }
    }

    /// <summary>
    /// Discrete state machine definition shared by every item instance that
    /// references it via <see cref="ItemData.StateDefinitionId"/>.
    /// </summary>
    public sealed class StateDefinition
    {
        public string DefinitionId { get; }
        public IReadOnlyList<string> States { get; }
        public string InitialState { get; }
        public IReadOnlyList<StateTransition> Transitions { get; }

        public StateDefinition(
            string definitionId,
            IEnumerable<string> states,
            string initialState,
            IEnumerable<StateTransition> transitions)
        {
            if (string.IsNullOrEmpty(definitionId))
            {
                throw new ArgumentException("A state definition requires a non-empty id.", nameof(definitionId));
            }

            var stateList = states != null ? new List<string>(states) : null;

            if (stateList == null || stateList.Count == 0)
            {
                throw new ArgumentException("A state definition requires at least one state.", nameof(states));
            }

            if (!stateList.Contains(initialState))
            {
                throw new ArgumentException(
                    $"Initial state '{initialState}' is not one of the defined states.", nameof(initialState));
            }

            DefinitionId = definitionId;
            States = stateList;
            InitialState = initialState;
            Transitions = transitions != null
                ? (IReadOnlyList<StateTransition>)new List<StateTransition>(transitions)
                : Array.Empty<StateTransition>();
        }

        public bool TryGetTransition(string fromState, string trigger, out string toState)
        {
            for (int i = 0; i < Transitions.Count; i++)
            {
                StateTransition transition = Transitions[i];
                if (transition.FromState == fromState && transition.Trigger == trigger)
                {
                    toState = transition.ToState;
                    return true;
                }
            }

            toState = null;
            return false;
        }
    }
}
