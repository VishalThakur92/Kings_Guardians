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
            return go;
        }
    }
}
