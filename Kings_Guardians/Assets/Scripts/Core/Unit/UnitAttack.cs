using UnityEngine;
using KingGuardians.Combat;

namespace KingGuardians.Units
{
    /// <summary>
    /// MVP attack logic:
    /// - When given a target, schedules damage at fixed intervals.
    /// - IMPORTANT: Damage is ENQUEUED into DamageQueue, then applied in LateUpdate,
    ///   ensuring fair simultaneous resolution (no update-order bias).
    /// </summary>
    public sealed class UnitAttack : MonoBehaviour
    {
        [Header("Attack (MVP)")]
        [SerializeField] private int damagePerHit = 25;

        [Tooltip("Seconds between hits.")]
        [SerializeField] private float attackInterval = 0.75f;

        private IDamageable _target;
        private float _nextAttackTime;

        /// <summary>
        /// Called by UnitTargeter when unit engages something.
        /// </summary>
        public void SetTarget(IDamageable target)
        {
            _target = target;
            _nextAttackTime = Time.time; // hit immediately when in range
        }

        public void ClearTarget()
        {
            _target = null;
        }

        /// <summary>
        /// Applies combat values from stats (used by card/unit definitions).
        /// </summary>
        public void ApplyAttackStats(int damage, float interval)
        {
            damagePerHit = Mathf.Max(0, damage);
            attackInterval = Mathf.Max(0.05f, interval);
        }

        private void Update()
        {
            if (_target == null) return;
            if (!_target.IsAlive) return;

            if (Time.time < _nextAttackTime)
                return;

            // IMPORTANT:
            // Do not call TakeDamage directly. Enqueue for end-of-frame resolution.
            var dq = DamageQueue.Instance;
            if (dq != null)
            {
                dq.Enqueue(_target, damagePerHit);
            }
            else
            {
                // Fallback: if DamageQueue isn't present, apply immediately (not recommended).
                _target.TakeDamage(damagePerHit);
            }

            _nextAttackTime = Time.time + attackInterval;
        }
    }
}
