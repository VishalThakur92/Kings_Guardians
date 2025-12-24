using UnityEngine;

namespace KingGuardians.Core
{
    /// <summary>
    /// Composition root for deployment-related systems in the Battle scene.
    /// Keeps wiring explicit (no hidden Find calls).
    /// </summary>
    public sealed class DeploymentInstaller : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private BattlefieldConfig battlefieldConfig;

        [Header("Prefabs")]
        [SerializeField] private GameObject unitPlaceholderPrefab;

        [Header("Hierarchy")]
        [SerializeField] private Transform unitsRoot;

        [Header("Scene References")]
        [SerializeField] private DeploymentInput deploymentInput;

        private void Awake()
        {
            if (battlefieldConfig == null)
            {
                Debug.LogError("[DeploymentInstaller] Missing BattlefieldConfig reference.", this);
                enabled = false;
                return;
            }

            if (unitPlaceholderPrefab == null)
            {
                Debug.LogError("[DeploymentInstaller] Missing unitPlaceholderPrefab.", this);
                enabled = false;
                return;
            }

            if (deploymentInput == null)
            {
                Debug.LogError("[DeploymentInstaller] Missing DeploymentInput reference.", this);
                enabled = false;
                return;
            }

            if (unitsRoot == null)
            {
                // Default to this object as root if none provided.
                unitsRoot = this.transform;
            }

            // Build dependencies
            var validator = new DeploymentValidator(battlefieldConfig);
            var spawner = new UnitSpawner(unitPlaceholderPrefab, unitsRoot);

            // Inject into input handler
            deploymentInput.Initialize(battlefieldConfig, validator, spawner);
        }
    }
}
