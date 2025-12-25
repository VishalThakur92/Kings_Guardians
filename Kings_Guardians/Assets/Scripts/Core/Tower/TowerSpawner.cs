using UnityEngine;
using KingGuardians.Towers;

namespace KingGuardians.Core
{
    /// <summary>
    /// MVP tower spawner:
    /// - Spawns towers using BattlefieldConfig positions
    /// - Ensures TowerAnchor (identity) and TowerHealth (HP) exist
    /// - Configures trigger colliders for detection
    /// </summary>
    public sealed class TowerSpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject mainTowerPrefab;
        [SerializeField] private GameObject outpostPrefab;

        [Header("Hierarchy")]
        [SerializeField] private Transform towersRoot;

        [Header("MVP HP")]
        [SerializeField] private int outpostHp = 150;
        [SerializeField] private int mainTowerHp = 300;

        // Add this near other serialized fields
        [SerializeField] private GameObject worldHealthBarPrefab;
        [SerializeField] private Vector3 healthBarOffset = new Vector3(0f, 1.2f, 0f);


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
            SpawnTower(mainTowerPrefab, _cfg.PlayerMainTowerPos, "P_MainTower", TeamId.Player, TowerType.Main, mainTowerHp);
            SpawnTower(outpostPrefab, _cfg.PlayerOutpostAPos, "P_Outpost_A", TeamId.Player, TowerType.Outpost, outpostHp);
            SpawnTower(outpostPrefab, _cfg.PlayerOutpostBPos, "P_Outpost_B", TeamId.Player, TowerType.Outpost, outpostHp);

            // Enemy (top)
            SpawnTower(mainTowerPrefab, _cfg.EnemyMainTowerPos, "E_MainTower", TeamId.Enemy, TowerType.Main, mainTowerHp);
            SpawnTower(outpostPrefab, _cfg.EnemyOutpostAPos, "E_Outpost_A", TeamId.Enemy, TowerType.Outpost, outpostHp);
            SpawnTower(outpostPrefab, _cfg.EnemyOutpostBPos, "E_Outpost_B", TeamId.Enemy, TowerType.Outpost, outpostHp);
        }

        private void SpawnTower(GameObject prefab, Vector2 pos, string name, TeamId team, TowerType type, int hp)
        {
            if (prefab == null)
            {
                Debug.LogWarning($"[TowerSpawner] Missing prefab for {name}.", this);
                return;
            }

            var go = Instantiate(prefab, new Vector3(pos.x, pos.y, 0f), Quaternion.identity, towersRoot);
            go.name = name;

            // Identity
            var anchor = go.GetComponent<TowerAnchor>();
            if (anchor == null) anchor = go.AddComponent<TowerAnchor>();
            anchor.Init(team, type);

            // HP
            var health = go.GetComponent<TowerHealth>();
            if (health == null) health = go.AddComponent<TowerHealth>();
            health.Init(team, type, hp);

            AttachHealthBar(go.transform);


            // Trigger collider for "in range" detection
            EnsureTriggerCollider(go, type);
        }

        private void AttachHealthBar(Transform parent)
        {
            if (worldHealthBarPrefab == null) return;

            var hb = Instantiate(worldHealthBarPrefab, parent);
            hb.transform.localPosition = healthBarOffset;
        }


        private void EnsureTriggerCollider(GameObject go, TowerType type)
        {
            var col = go.GetComponent<CircleCollider2D>();
            if (col == null) col = go.AddComponent<CircleCollider2D>();

            col.isTrigger = true;
            //col.radius = (type == TowerType.Main) ? 23f : 7f;
        }
    }
}
