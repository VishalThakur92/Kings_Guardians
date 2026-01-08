using UnityEngine;
using KingGuardians.Core;

namespace KingGuardians.Units
{
    /// <summary>
    /// Minimal identity for a unit (currently only team).
    /// Towers use this to decide who is an enemy.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UnitIdentity : MonoBehaviour
    {
        [SerializeField] private TeamId team = TeamId.Player;

        public TeamId Team => team;

        /// <summary>
        /// Runtime initialization (spawner sets team).
        /// </summary>
        public void Init(TeamId newTeam) => team = newTeam;
    }
}
