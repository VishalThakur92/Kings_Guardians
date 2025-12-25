using UnityEngine;
using KingGuardians.Core;

namespace KingGuardians.Towers
{
    /// <summary>
    /// Minimal tower identity used by targeting.
    /// TowerHealth handles HP/damage separately.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TowerAnchor : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField] private TeamId team = TeamId.Enemy;
        [SerializeField] private TowerType towerType = TowerType.Outpost;

        public TeamId Team => team;
        public TowerType Type => towerType;

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
