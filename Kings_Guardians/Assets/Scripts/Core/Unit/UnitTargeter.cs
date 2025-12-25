using UnityEngine;
using KingGuardians.Core;
using KingGuardians.Towers;

namespace KingGuardians.Units
{
    /// <summary>
    /// MVP targeting:
    /// - Detects enemy towers via trigger entry.
    /// - Stops UnitMotor when in range of the first enemy structure reached.
    ///
    /// Later this becomes:
    /// - target selection rules
    /// - attack range checks
    /// - combat state machine
    /// </summary>
    [RequireComponent(typeof(UnitMotor))]
    public sealed class UnitTargeter : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField] private TeamId team = TeamId.Player;

        private UnitMotor _motor;

        private TowerAnchor _currentTarget;

        private void Awake()
        {
            _motor = GetComponent<UnitMotor>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Only care about towers
            if (!other.TryGetComponent<TowerAnchor>(out var tower))
                return;

            // Ignore friendly towers
            if (tower.Team == team)
                return;

            // If we already have a target, keep the first one we reached (MVP rule).
            if (_currentTarget != null)
                return;

            _currentTarget = tower;

            // Stop movement when we reach the first enemy tower.
            _motor.Stop();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // If we ever move away in future logic, we could resume.
            // For MVP, units stop and stay.
        }
    }
}
