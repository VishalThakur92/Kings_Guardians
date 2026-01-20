using UnityEngine;
using KingGuardians.Combat;

namespace KingGuardians.Units
{
    /// <summary>
    /// Melee attack behavior:
    /// - Only attacks if target is within range
    /// - Applies damage instantly (no projectile)
    /// - Enforces attack interval cooldown
    ///
    /// SRP: This class only handles melee attack execution.
    /// </summary>
    public sealed class MeleeAttack : MonoBehaviour, IAttackBehavior
    {
        [Header("Runtime Stats")]
        [SerializeField] private int damagePerHit = 25;
        [SerializeField] private float attackInterval = 0.75f;
        [SerializeField] private float attackRange = 0.9f;

        private IDamageable _target;
        private Transform _targetTransform;

        private float _nextAttackTime;

        public void SetTarget(IDamageable target)
        {
            _target = target;
            _targetTransform = (target as Component) != null ? ((Component)target).transform : null;

            // Allow immediate hit once in range.
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
            if (_targetTransform == null) return true; // fallback: assume in range

            float distSq = (_targetTransform.position - transform.position).sqrMagnitude;
            float rangeSq = attackRange * attackRange;
            return distSq <= rangeSq;
        }

        public void Tick(float deltaTime)
        {
            if (_target == null || !_target.IsAlive) return;
            if (!IsTargetInRange()) return;

            if (Time.time < _nextAttackTime) return;

            // Instant melee hit
            _target.TakeDamage(damagePerHit);
            _nextAttackTime = Time.time + attackInterval;
        }
    }
}
