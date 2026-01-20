using UnityEngine;
using KingGuardians.Combat;

namespace KingGuardians.Units
{
    /// <summary>
    /// Ranged attack wrapper:
    /// - When a target is set, it fires a projectile at fixed intervals
    /// - Projectile handles travel + applying damage
    ///
    /// This mirrors UnitAttack (melee) but uses projectile delivery.
    /// </summary>
    public sealed class RangedAttack : MonoBehaviour
    {
        [Header("Attack (MVP)")]
        [SerializeField] private int damagePerHit = 20;
        [Min(0.05f)][SerializeField] private float attackInterval = 0.8f;

        [Header("Projectile")]
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private Transform firePoint; // optional; defaults to this transform

        private IDamageable _target;
        private Transform _targetTransform;

        private float _nextShotTime;

        private void Awake()
        {
            if (firePoint == null)
                firePoint = transform;
        }

        /// <summary>
        /// Apply stats from UnitStatsDefinition.
        /// </summary>
        public void ApplyAttackStats(int damage, float interval)
        {
            damagePerHit = Mathf.Max(0, damage);
            attackInterval = Mathf.Max(0.05f, interval);
        }

        public void SetTarget(IDamageable target, Transform targetTransform)
        {
            _target = target;
            _targetTransform = targetTransform;
            _nextShotTime = Time.time; // fire immediately on engage
        }

        public void ClearTarget()
        {
            _target = null;
            _targetTransform = null;
        }

        private void Update()
        {
            if (_target == null || !_target.IsAlive) return;
            if (projectilePrefab == null) return;

            if (Time.time >= _nextShotTime)
            {
                Fire();
                _nextShotTime = Time.time + attackInterval;
            }
        }

        private void Fire()
        {
            // Spawn projectile at fire point
            var proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            // Init with both transform (for tracking) and damageable (for damage)
            proj.Init(_targetTransform, _target, damagePerHit);
        }
    }
}
