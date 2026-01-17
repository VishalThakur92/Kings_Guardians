using UnityEngine;
using KingGuardians.Combat;
using KingGuardians.Towers;

namespace KingGuardians.Units
{
    /// <summary>
    /// MVP attack logic:
    /// - When given a target (TowerHealth), deals damage every cooldown interval.
    /// - No animations yet; purely logical.
    /// </summary>
    public sealed class UnitAttack : MonoBehaviour
    {
        [Header("Attack (MVP)")]
        [SerializeField] private int damagePerHit = 25;

        [Tooltip("Seconds between hits.")]
        [SerializeField] private float attackInterval = 0.75f;

        private IDamageable _target;
        private float _nextAttackTime;

        public void SetTarget(IDamageable target)
        {
            _target = target;
            _nextAttackTime = Time.time; // hit immediately on engage
        }

        public void ClearTarget()
        {
            _target = null;
        }

        /// <summary>
        /// Applies combat values from stats.
        /// </summary>
        public void ApplyAttackStats(int damage, float interval)
        {
            damagePerHit = Mathf.Max(0, damage);
            attackInterval = Mathf.Max(0.05f, interval);
        }


        private void Update()
        {
            if (_target == null || !_target.IsAlive) return;

            if (Time.time >= _nextAttackTime)
            {
                _target.TakeDamage(damagePerHit);
                _nextAttackTime = Time.time + attackInterval;
            }
        }
    }
}
