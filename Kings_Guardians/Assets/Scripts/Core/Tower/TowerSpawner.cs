using UnityEngine;
using KingGuardians.Towers;

namespace KingGuardians.Core
{
    /// <summary>
    /// MVP tower spawner.
    /// Spawns towers/outposts using BattlefieldConfig positions and assigns team/type at runtime.
    /// </summary>
    public sealed class TowerSpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject mainTowerPrefab;
        [SerializeField] private GameObject outpostPrefab;

        [Header("Hierarchy")]
        [SerializeField] private Transform towersRoot;

        private BattlefieldConfig _cfg;

        public void Initialize(BattlefieldConfig cfg) => _cfg = cfg;

        public void SpawnAll()
        {
            if (_cfg == null)
            {
                Debug.LogError("[TowerSpawner] Initialize() not called.");
                return;
            }

            if (towersRoot == null) towersRoot = this.transform;

            // Player (bottom)
            SpawnTower(mainTowerPrefab, _cfg.PlayerMainTowerPos, "P_MainTower", TeamId.Player, TowerType.Main);
            SpawnTower(outpostPrefab, _cfg.PlayerOutpostAPos, "P_Outpost_A", TeamId.Player, TowerType.Outpost);
            SpawnTower(outpostPrefab, _cfg.PlayerOutpostBPos, "P_Outpost_B", TeamId.Player, TowerType.Outpost);

            // Enemy (top)
            SpawnTower(mainTowerPrefab, _cfg.EnemyMainTowerPos, "E_MainTower", TeamId.Enemy, TowerType.Main);
            SpawnTower(outpostPrefab, _cfg.EnemyOutpostAPos, "E_Outpost_A", TeamId.Enemy, TowerType.Outpost);
            SpawnTower(outpostPrefab, _cfg.EnemyOutpostBPos, "E_Outpost_B", TeamId.Enemy, TowerType.Outpost);
        }

        private void SpawnTower(GameObject prefab, Vector2 pos, string name, TeamId team, TowerType type)
        {
            if (prefab == null)
            {
                Debug.LogWarning($"[TowerSpawner] Missing prefab for {name}.", this);
                return;
            }

            var go = Instantiate(prefab, new Vector3(pos.x, pos.y, 0f), Quaternion.identity, towersRoot);
            go.name = name;

            // Ensure TowerAnchor exists and is configured
            var anchor = go.GetComponent<TowerAnchor>();
            if (anchor == null)
            {
                anchor = go.AddComponent<TowerAnchor>();
            }

            // Use reflection-free assignment via serialized backing fields? We kept them private.
            // So we configure by adding a small helper method (see below).
            ConfigureTowerAnchor(anchor, team, type);

            // Ensure trigger collider exists
            EnsureTriggerCollider(go, type);
        }

        private void ConfigureTowerAnchor(TowerAnchor anchor, TeamId team, TowerType type)
        {
            // TowerAnchor fields are serialized private; for MVP we keep it simple:
            // Use Unity's SerializedObject only in editor is messy; instead expose an Init().
            // We'll use an Init method by updating TowerAnchor accordingly.
            anchor.Init(team, type);
        }

        private void EnsureTriggerCollider(GameObject go, TowerType type)
        {
            var col = go.GetComponent<CircleCollider2D>();
            if (col == null) col = go.AddComponent<CircleCollider2D>();

            col.isTrigger = true;

            // Simple radii for MVP
            col.radius = (type == TowerType.Main) ? 23f : 8f;
        }
    }
}
