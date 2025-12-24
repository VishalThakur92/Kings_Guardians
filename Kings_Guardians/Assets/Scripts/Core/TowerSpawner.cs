using UnityEngine;

namespace KingGuardians.Core
{
    /// <summary>
    /// MVP-only spawner for towers/outposts.
    /// Later, this will be replaced by a proper match state + server-authoritative spawn pipeline.
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

            // Player A
            Spawn(mainTowerPrefab, _cfg.PlayerMainTowerPos, "P_MainTower");
            Spawn(outpostPrefab, _cfg.PlayerOutpostAPos, "P_Outpost_A");
            Spawn(outpostPrefab, _cfg.PlayerOutpostBPos, "P_Outpost_B");

            // Player B
            Spawn(mainTowerPrefab, _cfg.EnemyMainTowerPos, "E_MainTower");
            Spawn(outpostPrefab, _cfg.EnemyOutpostAPos, "E_Outpost_A");
            Spawn(outpostPrefab, _cfg.EnemyOutpostBPos, "E_Outpost_B");
        }

        private void Spawn(GameObject prefab, Vector2 pos, string name)
        {
            if (prefab == null)
            {
                Debug.LogWarning($"[TowerSpawner] Missing prefab for {name}.", this);
                return;
            }

            var go = Instantiate(prefab, new Vector3(pos.x, pos.y, 0f), Quaternion.identity, towersRoot);
            go.name = name;
        }
    }
}
