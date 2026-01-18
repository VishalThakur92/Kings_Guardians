using System.Collections.Generic;
using UnityEngine;
using KingGuardians.Core;
using KingGuardians.Units;

namespace KingGuardians.Towers
{
    /// <summary>
    /// MVP tower attack system:
    /// - Uses the tower's trigger collider as "attack range"
    /// - Tracks enemy units entering/leaving range
    /// - Deals damage at a fixed interval
    ///
    /// Visual projectiles are not required for MVP; this is logical damage only.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TowerAttack : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField] private TeamId team = TeamId.Enemy;

        [Header("Attack (MVP)")]
        [SerializeField] private int damagePerHit = 10;

        [Tooltip("Seconds between shots.")]
        [SerializeField] private float attackInterval = 0.75f;

        private readonly List<UnitHealth> _targetsInRange = new List<UnitHealth>(16);
        private float _nextShotTime;

        /// <summary>
        /// Runtime init (spawner sets team). Keeps prefabs generic.
        /// </summary>
        public void Init(TeamId newTeam)
        {
            team = newTeam;
        }



        /// <summary>
        /// Allows spawner to set damage and interval at runtime.
        /// </summary>
        public void Configure(int damage, float interval)
        {
            damagePerHit = Mathf.Max(0, damage);
            attackInterval = Mathf.Max(0.05f, interval);
        }

        private void Update()
        {
            if (Time.time < _nextShotTime)
                return;

            CleanupDeadTargets();

            var target = SelectTarget();
            if (target == null)
                return;

            //target.TakeDamage(damagePerHit);

            var dq = Combat.DamageQueue.Instance;

            if (dq != null) 
                dq.Enqueue(target, damagePerHit);
            else 
                target.TakeDamage(damagePerHit);


            _nextShotTime = Time.time + attackInterval;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // We only care about units with identity + health.
            if (!other.TryGetComponent<UnitIdentity>(out var identity))
                return;

            if (!other.TryGetComponent<UnitHealth>(out var health))
                return;

            // Only attack enemies.
            if (identity.Team == team)
                return;

            if (!_targetsInRange.Contains(health))
                _targetsInRange.Add(health);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.TryGetComponent<UnitHealth>(out var health))
                return;

            _targetsInRange.Remove(health);
        }

        private UnitHealth SelectTarget()
        {
            // MVP targeting rule: pick the closest unit to this tower (by distance).
            // Simple, predictable, cheap.
            UnitHealth best = null;
            float bestDistSq = float.MaxValue;

            Vector3 towerPos = transform.position;

            for (int i = 0; i < _targetsInRange.Count; i++)
            {
                var h = _targetsInRange[i];
                if (h == null || !h.IsAlive) continue;

                float d = (h.transform.position - towerPos).sqrMagnitude;
                if (d < bestDistSq)
                {
                    bestDistSq = d;
                    best = h;
                }
            }

            return best;
        }

        private void CleanupDeadTargets()
        {
            // Remove destroyed references and dead units (prevents list growth).
            for (int i = _targetsInRange.Count - 1; i >= 0; i--)
            {
                var h = _targetsInRange[i];
                if (h == null || !h.IsAlive)
                    _targetsInRange.RemoveAt(i);
            }
        }

    }
}
