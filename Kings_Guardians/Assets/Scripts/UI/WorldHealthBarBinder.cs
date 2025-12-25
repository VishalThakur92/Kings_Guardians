using UnityEngine;
using KingGuardians.Combat;

namespace KingGuardians.UI
{
    /// <summary>
    /// Attaches to a HealthBar prefab instance.
    /// Finds IHealthReadable on the parent (tower/unit) and binds automatically.
    /// </summary>
    [RequireComponent(typeof(WorldHealthBar))]
    public sealed class WorldHealthBarBinder : MonoBehaviour
    {
        private void Start()
        {
            var bar = GetComponent<WorldHealthBar>();

            // Look up health component in parent hierarchy
            var health = GetComponentInParent<MonoBehaviour>() as IHealthReadable;

            // The line above won't reliably find interface types.
            // Use explicit search:
            IHealthReadable found = FindHealthInParents();
            if (found == null)
            {
                Debug.LogWarning("[WorldHealthBarBinder] No IHealthReadable found in parents.", this);
                return;
            }

            bar.Bind(found);
        }

        private IHealthReadable FindHealthInParents()
        {
            Transform t = transform.parent;
            while (t != null)
            {
                // Get all MonoBehaviours and test interface
                var behaviours = t.GetComponents<MonoBehaviour>();
                foreach (var b in behaviours)
                {
                    if (b is IHealthReadable readable)
                        return readable;
                }

                t = t.parent;
            }
            return null;
        }
    }
}
