using KingGuardians.Combat;
using KingGuardians.Core;
using KingGuardians.Towers;
using System.Collections.Generic;
using UnityEngine;

namespace KingGuardians.Units
{
    /// <summary>
    /// MVP Targeting Rules (Phase 2):
    /// 1) If any enemy UNITS are within trigger range -> attack the closest enemy unit.
    /// 2) Else if any enemy TOWERS are within trigger range -> attack the closest enemy tower.
    /// 3) Else -> move forward (UnitMotor drives movement).
    ///
    /// This is intentionally simple and deterministic. It creates "frontlines" and prevents
    /// units from ignoring each other.
    ///
    /// Requirements for triggers to work reliably in Unity 2D:
    /// - Units must have a Rigidbody2D (Kinematic is fine) + a Trigger Collider.
    /// - Towers must have Trigger colliders (your spawner already ensures this).
    /// </summary>
    [RequireComponent(typeof(UnitMotor))]
    [RequireComponent(typeof(UnitAttack))]
    public sealed class UnitTargeter : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField] private TeamId team = TeamId.Player;

        [Header("Optional Tuning")]
        [Tooltip("If true, unit prioritizes enemy units over towers (recommended).")]
        [SerializeField] private bool prioritizeUnitsOverTowers = true;

        private UnitMotor _motor;
        private UnitAttack _attack;

        // Tracking who is currently inside this unit's trigger range.
        private readonly List<UnitHealth> _enemyUnitsInRange = new List<UnitHealth>(8);
        private readonly List<TowerHealth> _enemyTowersInRange = new List<TowerHealth>(4);

        // Current chosen target (could be unit or tower)
        private IDamageable _currentTarget;

        private void Awake()
        {
            _motor = GetComponent<UnitMotor>();
            _attack = GetComponent<UnitAttack>();
        }

        /// <summary>
        /// Allows spawners to set team at runtime (Player/Enemy).
        /// Must be called for enemy spawns.
        /// </summary>
        public void SetTeam(TeamId newTeam)
        {
            team = newTeam;
        }

        private void Update()
        {
            // Clean out dead/null references so we don't target destroyed objects.
            CleanupLists();

            // Decide best target based on priority rules.
            IDamageable best = ChooseBestTarget();

            // If target changed, update motor + attack.
            if (!ReferenceEquals(best, _currentTarget))
            {
                _currentTarget = best;

                if (_currentTarget == null)
                {
                    // Nothing to fight -> keep moving.
                    _attack.ClearTarget();
                    _motor.Resume();
                }
                else
                {
                    // Engage -> stop and attack.
                    _motor.Stop();
                    _attack.SetTarget(_currentTarget);
                }
            }
        }

        private IDamageable ChooseBestTarget()
        {
            // 1) Prefer enemy units if enabled and any exist in range.
            if (prioritizeUnitsOverTowers)
            {
                var unitTarget = ClosestAliveUnit();
                if (unitTarget != null)
                    return unitTarget;
            }

            // 2) Otherwise, target a tower if any exist in range.
            var towerTarget = ClosestAliveTower();
            if (towerTarget != null)
                return towerTarget;

            // 3) If units are not prioritized, we still may want to fight units if no tower is in range.
            if (!prioritizeUnitsOverTowers)
            {
                var unitTarget = ClosestAliveUnit();
                if (unitTarget != null)
                    return unitTarget;
            }

            return null;
        }

        private UnitHealth ClosestAliveUnit()
        {
            UnitHealth best = null;
            float bestDistSq = float.MaxValue;
            Vector3 pos = transform.position;

            for (int i = 0; i < _enemyUnitsInRange.Count; i++)
            {
                var u = _enemyUnitsInRange[i];
                if (u == null || !u.IsAlive) continue;

                float d = (u.transform.position - pos).sqrMagnitude;
                if (d < bestDistSq)
                {
                    bestDistSq = d;
                    best = u;
                }
            }

            return best;
        }

        private TowerHealth ClosestAliveTower()
        {
            TowerHealth best = null;
            float bestDistSq = float.MaxValue;
            Vector3 pos = transform.position;

            for (int i = 0; i < _enemyTowersInRange.Count; i++)
            {
                var t = _enemyTowersInRange[i];
                if (t == null || !t.IsAlive) continue;

                float d = (t.transform.position - pos).sqrMagnitude;
                if (d < bestDistSq)
                {
                    bestDistSq = d;
                    best = t;
                }
            }

            return best;
        }

        private void CleanupLists()
        {
            for (int i = _enemyUnitsInRange.Count - 1; i >= 0; i--)
            {
                var u = _enemyUnitsInRange[i];
                if (u == null || !u.IsAlive)
                    _enemyUnitsInRange.RemoveAt(i);
            }

            for (int i = _enemyTowersInRange.Count - 1; i >= 0; i--)
            {
                var t = _enemyTowersInRange[i];
                if (t == null || !t.IsAlive)
                    _enemyTowersInRange.RemoveAt(i);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // ---- Enemy Unit detection ----
            // Unit must have UnitIdentity to determine if it's enemy.
            if (other.TryGetComponent<UnitIdentity>(out var identity) &&
                other.TryGetComponent<UnitHealth>(out var unitHealth))
            {
                // Ignore friendly units.
                if (identity.Team == team) return;

                // Track enemy unit if not already tracked.
                if (!_enemyUnitsInRange.Contains(unitHealth))
                    _enemyUnitsInRange.Add(unitHealth);

                return; // A collider can also have other components; unit takes precedence.
            }

            // ---- Enemy Tower detection ----
            if (other.TryGetComponent<TowerAnchor>(out var towerAnchor) &&
                other.TryGetComponent<TowerHealth>(out var towerHealth))
            {
                // Ignore friendly towers.
                if (towerAnchor.Team == team) return;

                if (!_enemyTowersInRange.Contains(towerHealth))
                    _enemyTowersInRange.Add(towerHealth);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Remove enemy unit if leaving range
            if (other.TryGetComponent<UnitHealth>(out var unitHealth))
            {
                _enemyUnitsInRange.Remove(unitHealth);
            }

            // Remove tower if leaving range
            if (other.TryGetComponent<TowerHealth>(out var towerHealth))
            {
                _enemyTowersInRange.Remove(towerHealth);
            }
        }
    }
}
