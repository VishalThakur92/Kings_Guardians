using UnityEngine;
using KingGuardians.Core;

namespace KingGuardians.Towers
{
    /// <summary>
    /// Minimal tower identity + detection radius for MVP.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TowerAnchor : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField] private TeamId team = TeamId.Enemy;
        [SerializeField] private TowerType towerType = TowerType.Outpost;

        public TeamId Team => team;
        public TowerType Type => towerType;

        /// <summary>
        /// Runtime initialization (spawner assigns correct values).
        /// Keeps prefabs generic and avoids duplicating prefabs for teams.
        /// </summary>
        public void Init(TeamId newTeam, TowerType newType)
        {
            team = newTeam;
            towerType = newType;
        }

        private void Reset()
        {
            var col = GetComponent<Collider2D>();
            if (col != null) col.isTrigger = true;
        }
    }
}
