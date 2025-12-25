using UnityEngine;
using KingGuardians.Core;
using KingGuardians.Towers;

namespace KingGuardians.Units
{
    /// <summary>
    /// MVP targeting:
    /// - Detect enemy towers via trigger entry
    /// - Stop movement
    /// - Set UnitAttack target to the tower's TowerHealth
    /// </summary>
    [RequireComponent(typeof(UnitMotor))]
    [RequireComponent(typeof(UnitAttack))]
    public sealed class UnitTargeter : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField] private TeamId team = TeamId.Player;

        private UnitMotor _motor;
        private UnitAttack _attack;

        private TowerAnchor _currentTargetAnchor;
        private TowerHealth _currentTargetHealth;

        private void Awake()
        {
            _motor = GetComponent<UnitMotor>();
            _attack = GetComponent<UnitAttack>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<TowerAnchor>(out var towerAnchor))
                return;

            // Ignore friendly towers
            if (towerAnchor.Team == team)
                return;

            // Keep first target reached (MVP)
            if (_currentTargetAnchor != null)
                return;

            // Require TowerHealth for combat
            if (!other.TryGetComponent<TowerHealth>(out var towerHealth))
                return;

            _currentTargetAnchor = towerAnchor;
            _currentTargetHealth = towerHealth;

            // Stop and engage
            _motor.Stop();
            _attack.SetTarget(towerHealth);
        }

        private void Update()
        {
            // If our target died, clear target.
            if (_currentTargetHealth != null && !_currentTargetHealth.IsAlive)
            {
                _attack.ClearTarget();
                _currentTargetHealth = null;
                _currentTargetAnchor = null;

                // MVP behavior: resume forward movement after destroying an outpost
                _motor.Resume();
            }
        }
    }
}
