using UnityEngine;

namespace KingGuardians.Core
{
    /// <summary>
    /// Composition Root for the Battle scene.
    /// Wires config + scene references.
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

            battlefieldGizmos.Initialize(battlefieldConfig);

            towerSpawner.Initialize(battlefieldConfig);
            towerSpawner.SpawnAll();
        }
    }
}
