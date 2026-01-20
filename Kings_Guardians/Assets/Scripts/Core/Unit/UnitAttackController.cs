using UnityEngine;
using KingGuardians.Combat;

namespace KingGuardians.Units
{
    /// <summary>
    /// Attack coordinator.
    ///
    /// Why this version exists:
    /// Some projects accidentally end up with multiple IAttackBehavior interface definitions
    /// (same name, different assembly/namespace), causing casts like:
    ///   (mono as IAttackBehavior)
    /// to fail even though the class "implements" an interface.
    ///
    /// This controller avoids that entire class of bugs by:
    /// - referencing concrete components directly (MeleeAttack, RangedAttack)
    /// - still delegating behavior and keeping responsibilities separated
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UnitAttackController : MonoBehaviour
    {
        [Header("Behaviors (auto-wired if empty)")]
        [SerializeField] private MeleeAttack melee;
        [SerializeField] private RangedAttack ranged;

        // Active behavior reference (uses the SINGLE interface from KingGuardians.Combat)
        private IAttackBehavior _active;

        private void Awake()
        {
            // Auto-wire from same GameObject
            if (melee == null) melee = GetComponent<MeleeAttack>();
            if (ranged == null) ranged = GetComponent<RangedAttack>();

            // Default to melee (ranged is opt-in)
            _active = melee;

            if (_active == null && ranged != null)
                _active = ranged;

            if (_active == null)
            {
                Debug.LogError("[UnitAttackController] No MeleeAttack or RangedAttack found on this unit.", this);
            }
        }

        /// <summary>
        /// Selects which attack behavior to use.
        /// </summary>
        public void SetIsRanged(bool isRanged)
        {
            if (isRanged)
                _active = (ranged != null) ? (IAttackBehavior)ranged : (IAttackBehavior)melee;
            else
                _active = (melee != null) ? (IAttackBehavior)melee : (IAttackBehavior)ranged;

            if (_active == null)
                Debug.LogError("[UnitAttackController] Active attack behavior is null after SetIsRanged().", this);
        }

        public void SetTarget(IDamageable target) => _active?.SetTarget(target);
        public void ClearTarget() => _active?.SetTarget(null);
        public bool IsTargetInRange() => _active != null && _active.IsTargetInRange();
        public void Tick(float dt) => _active?.Tick(dt);

        /// <summary>
        /// Applies the same stats to both behaviors so switching is safe.
        /// </summary>
        public void ApplyStats(int damage, float interval, float range)
        {
            melee?.ApplyStats(damage, interval, range);
            ranged?.ApplyStats(damage, interval, range);
        }
    }
}
