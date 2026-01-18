using System;

namespace KingGuardians.Units
{
    /// <summary>
    /// Simple bitmask defining what a unit can target.
    /// MVP: ground and/or air.
    /// </summary>
    [Flags]
    public enum TargetMask
    {
        None = 0,
        Ground = 1 << 0,
        Air = 1 << 1,
        Both = Ground | Air
    }
}
