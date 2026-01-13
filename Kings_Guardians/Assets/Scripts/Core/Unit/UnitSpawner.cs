using UnityEngine;

namespace KingGuardians.Core
{
    /// <summary>
    /// Responsible only for instantiating unit prefabs.
    /// Does not decide *where* or *whether* to spawn (SOLID).
    /// </summary>
    public sealed class UnitSpawner
    {
        private readonly Transform _root;

        public UnitSpawner(Transform root)
        {
            _root = root;
        }

        public GameObject Spawn(GameObject prefab, Vector2 worldPos, string namePrefix = "Unit")
        {
            if (prefab == null)
            {
                Debug.LogError("[UnitSpawner] Prefab is null.");
                return null;
            }

            var go = Object.Instantiate(prefab, new Vector3(worldPos.x, worldPos.y, 0f), Quaternion.identity, _root);
            go.name = $"{namePrefix}_{go.GetInstanceID()}";

            // Ensure identity exists and is set to Player for MVP spawns.
            var identity = go.GetComponent<KingGuardians.Units.UnitIdentity>();
            if (identity == null) identity = go.AddComponent<KingGuardians.Units.UnitIdentity>();
            identity.Init(KingGuardians.Core.TeamId.Player);

            // Ensure health exists so towers can damage units and the health bar can bind.
            var health = go.GetComponent<KingGuardians.Units.UnitHealth>();
            if (health == null) health = go.AddComponent<KingGuardians.Units.UnitHealth>();

            return go;
        }

    }
}
