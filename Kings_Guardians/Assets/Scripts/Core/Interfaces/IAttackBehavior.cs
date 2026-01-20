using UnityEngine;
using KingGuardians.Combat;

namespace KingGuardians.Combat
{
    /// <summary>
    /// Strategy interface for attack execution (SOLID: Strategy pattern).
    /// UnitAttackController depends on this abstraction, not concrete melee/ranged implementations.
    /// </summary>
    public interface IAttackBehavior
    {
        /// <summary>
        /// Sets/clears the target that this behavior should attack.
        /// </summary>
        void SetTarget(IDamageable target);

        /// <summary>
        /// Configures behavior stats (damage, interval, range).
        /// Each behavior may interpret these differently, but must honor the meaning.
        /// </summary>
        void ApplyStats(int damagePerHit, float attackInterval, float attackRange);

        /// <summary>
        /// Returns true if the current target is within attack range.
        /// Used by UnitTargeter to decide whether to stop or keep moving.
        /// </summary>
        bool IsTargetInRange();

        /// <summary>
        /// Executes attack logic each frame (cooldowns, projectile firing, etc).
        /// </summary>
        void Tick(float deltaTime);
    }
}
