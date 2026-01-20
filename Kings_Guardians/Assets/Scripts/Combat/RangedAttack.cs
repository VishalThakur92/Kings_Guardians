using UnityEngine;
using KingGuardians.Combat;

namespace KingGuardians.Units
{
    /// <summary>
    /// Ranged attack behavior:
    /// - Only attacks if target is within range
    /// - Spawns a projectile prefab which homes to the target and applies damage on hit
    /// - Enforces attack interval cooldown
    ///
    /// SRP: This class only handles ranged attack execution and projectile spawning.
    /// </summary>
    public sealed class RangedAttack : MonoBehaviour, IAttackBehavior
    {
        [Header("Runtime Stats")]
        [SerializeField] private int damagePerHit = 20;
        [SerializeField] private float attackInterval = 0.9f;
        [SerializeField] private float attackRange = 3.0f;

        [Header("Projectile")]
        [Tooltip("Prefab with a Projectile component (and SpriteRenderer).")]
        [SerializeField] private Projectile projectilePrefab;

        [Tooltip("Optional fire point. If null, projectile spawns at this transform.")]
        [SerializeField] private Transform firePoint;

        private IDamageable _target;
        private Transform _targetTransform;

        private float _nextAttackTime;

        public void SetTarget(IDamageable target)
        {
            _target = target;
            _targetTransform = (target as Component) != null ? ((Component)target).transform : null;
            _nextAttackTime = Time.time;
        }

        public void ApplyStats(int damage, float interval, float range)
        {
            damagePerHit = Mathf.Max(0, damage);
            attackInterval = Mathf.Max(0.05f, interval);
            attackRange = Mathf.Max(0.1f, range);
        }

        public bool IsTargetInRange()
        {
            if (_target == null || !_target.IsAlive) return false;
            if (_targetTransform == null) return true;

            float distSq = (_targetTransform.position - transform.position).sqrMagnitude;
            float rangeSq = attackRange * attackRange;
            return distSq <= rangeSq;
        }

        public void Tick(float deltaTime)
        {
            if (_target == null || !_target.IsAlive) return;
            if (_targetTransform == null) return; // ranged must know where to shoot

            if (!IsTargetInRange()) return;
            if (Time.time < _nextAttackTime) return;

            FireProjectile();
            _nextAttackTime = Time.time + attackInterval;
        }

        private void FireProjectile()
        {
            if (projectilePrefab == null)
            {
                // If prefab is missing, you will "see nothing". This is the most common reason.
                Debug.LogWarning("[RangedAttack] projectilePrefab is not assigned.", this);
                return;
            }

            Transform spawnT = firePoint != null ? firePoint : transform;

            // Instantiate projectile and initialize it.
            Projectile proj = Instantiate(projectilePrefab, spawnT.position, Quaternion.identity);

            // Your Projectile.cs supports Init(Transform, IDamageable, int).
            // This wires homing + damage delivery on hit.
            proj.Init(_targetTransform, _target, damagePerHit);
        }
    }
}
