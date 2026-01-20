using KingGuardians.Core;
using System.Security.Principal;
using UnityEngine;

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

        /// <summary>
        /// Always resolves UnitIdentity when needed.
        /// This avoids bugs where UnitIdentity is added after Awake().
        /// </summary>
        private UnitIdentity Identity
        {
            get
            {
                if (_identity == null)
                    _identity = GetComponent<UnitIdentity>();
                return _identity;
            }
        }

        public UnitDomain Domain => domain;
        public TargetMask CanTarget => canTarget;

        private void Awake()
        {
            _identity = GetComponent<UnitIdentity>();
        }
        /// <summary>
        /// Team is sourced from UnitIdentity.
        /// If identity is missing, default to Player (safe fallback).
        /// </summary>
        public TeamId Team => Identity != null ? Identity.Team : TeamId.Player;


        /// <summary>
        /// Applies stats-driven descriptor values at spawn time.
        /// </summary>
        public void Apply(UnitDomain newDomain, TargetMask newMask)
        {
            domain = newDomain;
            canTarget = newMask;
        }
    }
}
