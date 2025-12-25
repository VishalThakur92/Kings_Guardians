using System;

namespace KingGuardians.Combat
{
    /// <summary>
    /// Read-only health contract for UI and other systems.
    /// Prevents UI from depending on concrete health implementations.
    /// </summary>
    public interface IHealthReadable
    {
        int CurrentHp { get; }
        int MaxHp { get; }

        /// <summary>
        /// Fired whenever health changes. (current, max)
        /// </summary>
        event Action<int, int> OnHealthChanged;
    }
}
