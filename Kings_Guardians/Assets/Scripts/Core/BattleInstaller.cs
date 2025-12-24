using UnityEngine;

namespace KingGuardians.Core
{
    /// <summary>
    /// Composition Root for the Battle scene.
    /// This is where we wire up dependencies (SOLID: Dependency Inversion).
    /// Avoid GameObject.Find and hidden scene coupling.
    /// </summary>
    public sealed class BattleInstaller : MonoBehaviour
    {
        [Header("Configs")]
        [SerializeField] private BattlefieldConfig battlefieldConfig;

        [Header("Scene References")]
        [SerializeField] private BattlefieldGizmos battlefieldGizmos;
        [SerializeField] private TowerSpawner towerSpawner;

        private void Awake()
        {
            // Validate scene wiring early to fail fast.
            if (battlefieldConfig == null)
            {
                Debug.LogError("[BattleInstaller] Missing BattlefieldConfig. Assign BattlefieldConfig_MVP.", this);
                enabled = false;
                return;
            }

            if (battlefieldGizmos == null)
            {
                Debug.LogError("[BattleInstaller] Missing BattlefieldGizmos reference.", this);
                enabled = false;
                return;
            }

            if (towerSpawner == null)
            {
                Debug.LogError("[BattleInstaller] Missing TowerSpawner reference.", this);
                enabled = false;
                return;
            }

            // Provide config to systems that need it.
            battlefieldGizmos.Initialize(battlefieldConfig);

            // Spawn towers for MVP (purely visual + placeholder objects).
            towerSpawner.Initialize(battlefieldConfig);
            towerSpawner.SpawnAll();
        }
    }
}
