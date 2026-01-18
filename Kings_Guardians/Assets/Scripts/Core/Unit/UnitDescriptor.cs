using UnityEngine;
using KingGuardians.Core;

namespace KingGuardians.Units
{
    /// <summary>
    /// Runtime descriptor for a unit:
    /// - Team (from UnitIdentity)
    /// - Domain (Ground/Air)
    /// - What it can target (TargetMask)
    ///
    /// Keeps targeting logic decoupled from ScriptableObjects.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UnitDescriptor : MonoBehaviour
    {
        [SerializeField] private UnitDomain domain = UnitDomain.Ground;
        [SerializeField] private TargetMask canTarget = TargetMask.Ground;

        private UnitIdentity _identity;

        public TeamId Team => _identity != null ? _identity.Team : TeamId.Player;
        public UnitDomain Domain => domain;
        public TargetMask CanTarget => canTarget;

        private void Awake()
        {
            _identity = GetComponent<UnitIdentity>();
        }

        /// <summary>
        /// Apply stats-driven descriptor values at spawn time.
        /// </summary>
        public void Apply(UnitDomain newDomain, TargetMask newMask)
        {
            domain = newDomain;
            canTarget = newMask;
        }
    }
}
