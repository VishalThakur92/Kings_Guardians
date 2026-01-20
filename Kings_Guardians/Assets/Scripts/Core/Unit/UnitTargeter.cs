using KingGuardians.Combat;
using KingGuardians.Core;
using KingGuardians.Towers;
using System.Collections.Generic;
using UnityEngine;
using KingGuardians.Units;


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
    public sealed class UnitTargeter : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField] private TeamId team = TeamId.Player;

        [Header("Optional Tuning")]
        [Tooltip("If true, unit prioritizes enemy units over towers (recommended).")]
        [SerializeField] private bool prioritizeUnitsOverTowers = true;

        private UnitMotor _motor; 
        private UnitAttackController _attack;


        // Tracking who is currently inside this unit's trigger range.
        private readonly List<UnitHealth> _enemyUnitsInRange = new List<UnitHealth>(8);
        private readonly List<TowerHealth> _enemyTowersInRange = new List<TowerHealth>(4);

        // Current chosen target (could be unit or tower)
        private IDamageable _currentTarget;

        private void Awake()
        {
            _motor = GetComponent<UnitMotor>();
            _attack = GetComponent<UnitAttackController>();
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

            // If target changed, update attack target.
            if (!ReferenceEquals(best, _currentTarget))
            {
                _currentTarget = best;

                if (_currentTarget == null)
                {
                    _attack.ClearTarget();
                    _motor.Resume();
                    return;
                }

                _attack.SetTarget(_currentTarget);
            }

            if (_currentTarget != null)
            {
                if (_attack.IsTargetInRange())
                    _motor.Stop();
                else
                    _motor.Resume();
            }
        }

        private Transform GetTargetTransform(IDamageable target)
        {
            // Most of your targets are MonoBehaviours, so we can safely get Transform.
            if (target is MonoBehaviour mb)
                return mb.transform;

            return null;
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
            // IMPORTANT:
            // Do NOT depend on UnitDescriptor existing, otherwise units may "ignore" each other
            // if one side spawns without a descriptor.
            // Use UnitIdentity for team check, and use UnitDescriptor only for domain filtering when present.
            if (other.TryGetComponent<UnitIdentity>(out var otherIdentity) &&
                other.TryGetComponent<UnitHealth>(out var unitHealth))
            {
                // Ignore friendly units.
                if (otherIdentity.Team == team) return;

                // Optional domain check:
                // If the other unit has a descriptor, respect domain targeting rules.
                if (other.TryGetComponent<UnitDescriptor>(out var otherDesc))
                {
                    if (!CanTargetDomain(otherDesc.Domain))
                        return;
                }
                else
                {
                    // If no descriptor exists, treat it as Ground for MVP safety.
                    if (!CanTargetDomain(UnitDomain.Ground))
                        return;
                }

                if (!_enemyUnitsInRange.Contains(unitHealth))
                    _enemyUnitsInRange.Add(unitHealth);

                return;
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

        private bool CanTargetDomain(UnitDomain targetDomain)
        {
            // If we don't have a descriptor, default to Ground targeting (safe MVP fallback).
            var selfDesc = GetComponent<UnitDescriptor>();
            if (selfDesc == null) return targetDomain == UnitDomain.Ground;

            if (targetDomain == UnitDomain.Ground) return (selfDesc.CanTarget & TargetMask.Ground) != 0;
            if (targetDomain == UnitDomain.Air) return (selfDesc.CanTarget & TargetMask.Air) != 0;

            return false;
        }

        private void LateUpdate()
        {
            // Let attack behavior tick independently

            if (_attack) {
                _attack.Tick(Time.deltaTime);
            }
        }


    }
}
