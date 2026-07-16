using System;

namespace EnriRanjan.Interaction
{
    /// <summary>
    /// What an item affords. Grabbable/Examinable are affordances consumed by
    /// the view layer; here they are plain data.
    /// </summary>
    [Flags]
    public enum ItemCapabilities
    {
        None = 0,
        Grabbable = 1 << 0,
        Examinable = 1 << 1,
        Combinable = 1 << 2,
        UsableOnTarget = 1 << 3,
        Storable = 1 << 4,
        Stateful = 1 << 5,
        Container = 1 << 6,
        NarrativeTrigger = 1 << 7
    }
}
