using UnityEngine;
using KingGuardians.Combat;

namespace KingGuardians.Combat
{
    /// <summary>
    /// Minimal homing projectile:
    /// - Moves towards a target Transform (or last known position if target is destroyed)
    /// - On reach (within hit radius), applies damage to IDamageable on the target object
    /// - Destroys itself after hit or if lifetime expires
    ///
    /// Designed to be simple and deterministic for MVP.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Projectile : MonoBehaviour
    {
        [Header("Tuning")]
        [Min(0.1f)][SerializeField] private float speed = 8f;
        [Min(0.01f)][SerializeField] private float hitRadius = 0.15f;
        [Min(0.1f)][SerializeField] private float maxLifetime = 3f;

        private Transform _targetTransform;
        private Vector3 _fallbackTargetPos;

        private IDamageable _damageableTarget;
        private int _damage;

        private float _spawnTime;

        /// <summary>
        /// Initializes projectile.
        /// Call immediately after Instantiate.
        /// </summary>
        public void Init(Transform target, IDamageable damageableTarget, int damage)
        {
            _targetTransform = target;
            _damageableTarget = damageableTarget;
            _damage = Mathf.Max(0, damage);

            _fallbackTargetPos = target != null ? target.position : transform.position;
            _spawnTime = Time.time;
        }

        private void Update()
        {
            // Lifetime safety
            if (Time.time - _spawnTime > maxLifetime)
            {
                Destroy(gameObject);
                return;
            }

            // Track target if still exists; otherwise keep moving to last known position
            if (_targetTransform != null)
                _fallbackTargetPos = _targetTransform.position;

            // Move towards target position
            Vector3 current = transform.position;
            Vector3 next = Vector3.MoveTowards(current, _fallbackTargetPos, speed * Time.deltaTime);
            transform.position = next;

            // Hit check
            float distSq = (next - _fallbackTargetPos).sqrMagnitude;
            if (distSq <= hitRadius * hitRadius)
            {
                // Apply damage if target is still valid and alive
                if (_damageableTarget != null && _damageableTarget.IsAlive && _damage > 0)
                    _damageableTarget.TakeDamage(_damage);

                Destroy(gameObject);
            }
        }
    }
}
