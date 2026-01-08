using UnityEngine;

namespace KingGuardians.Core
{
    /// <summary>
    /// Responsible only for instantiating unit prefabs.
    /// Does not decide *where* or *whether* to spawn (SOLID).
    /// </summary>
    public sealed class UnitSpawner
    {
        private readonly GameObject _unitPrefab;
        private readonly Transform _root;

        public UnitSpawner(GameObject unitPrefab, Transform root)
        {
            _unitPrefab = unitPrefab;
            _root = root;
        }
        public GameObject Spawn(Vector2 worldPos, string namePrefix = "Unit")
        {
            if (_unitPrefab == null)
            {
                Debug.LogError("[UnitSpawner] Unit prefab is null.");
                return null;
            }

            var go = Object.Instantiate(_unitPrefab, new Vector3(worldPos.x, worldPos.y, 0f), Quaternion.identity, _root);
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
