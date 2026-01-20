using UnityEngine;
using KingGuardians.Combat;

namespace KingGuardians.Units
{
    /// <summary>
    /// MVP attack logic:
    /// - Holds a current target (IDamageable)
    /// - Checks attack range before applying damage
    /// - Applies damage on a cooldown interval
    ///
    /// IMPORTANT:
    /// This version does NOT spawn projectiles yet. That comes in Step 4.2.
    /// For now, ranged units still "hit instantly", but only if within range.
    /// </summary>
    public sealed class UnitAttack : MonoBehaviour
    {
        [Header("Attack (MVP)")]
        [SerializeField] private int damagePerHit = 25;

        [Tooltip("Seconds between hits.")]
        [SerializeField] private float attackInterval = 0.75f;

        [Tooltip("World units attack range. Melee small, ranged larger.")]
        [SerializeField] private float attackRange = 0.8f;

        // Attack speed multiplier support (used by abilities if you added Rage).
        private float _attackSpeedMultiplier = 1f;
        private float _baseAttackInterval;

        // Current target (unit or tower) - interface only, no concrete coupling.
        private IDamageable _target;
        private Transform _targetTransform;

        private float _nextAttackTime;

        private void Awake()
        {
            // Treat inspector interval as base interval.
            _baseAttackInterval = attackInterval;
        }

        /// <summary>
        /// Called by targeting when a target is acquired.
        /// </summary>
        public void SetTarget(IDamageable target)
        {
            _target = target;

            // Cache Transform for distance checks (only if target is a Component).
            _targetTransform = (target as Component) != null ? ((Component)target).transform : null;

            // Allow immediate hit as soon as target is in range.
            _nextAttackTime = Time.time;
        }

        public void ClearTarget()
        {
            _target = null;
            _targetTransform = null;
        }

        /// <summary>
        /// Applies base stats from UnitStatsDefinition.
        /// </summary>
        public void ApplyAttackStats(int damage, float interval)
        {
            damagePerHit = Mathf.Max(0, damage);

            // Store base interval and update current interval with multiplier applied.
            _baseAttackInterval = Mathf.Max(0.05f, interval);
            attackInterval = Mathf.Max(0.05f, _baseAttackInterval / Mathf.Max(0.1f, _attackSpeedMultiplier));
        }

        /// <summary>
        /// Sets attack range from stats.
        /// </summary>
        public void ApplyAttackRange(float range)
        {
            attackRange = Mathf.Max(0.1f, range);
        }

        /// <summary>
        /// Temporary multiplier (1 = normal, 1.5 = faster).
        /// Keeps base interval intact.
        /// </summary>
        public void SetAttackSpeedMultiplier(float multiplier)
        {
            _attackSpeedMultiplier = Mathf.Max(0.1f, multiplier);
            attackInterval = Mathf.Max(0.05f, _baseAttackInterval / _attackSpeedMultiplier);
        }

        private void Update()
        {
            if (_target == null) return;
            if (!_target.IsAlive) return;

            // If we cannot check distance (no transform), fallback to always in-range.
            if (_targetTransform != null)
            {
                // Range gate: only attack if target is within range.
                float distSq = (_targetTransform.position - transform.position).sqrMagnitude;
                float rangeSq = attackRange * attackRange;

                if (distSq > rangeSq)
                    return;
            }

            // Cooldown gate
            if (Time.time < _nextAttackTime) return;

            // Apply damage
            _target.TakeDamage(damagePerHit);
            _nextAttackTime = Time.time + attackInterval;
        }
    }
}
