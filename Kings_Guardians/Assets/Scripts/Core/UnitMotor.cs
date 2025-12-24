using UnityEngine;

namespace KingGuardians.Units
{
    /// <summary>
    /// MVP movement component:
    /// - Moves the unit forward along +Y (portrait battlefield).
    /// - No targeting, no combat.
    /// - Kept simple and deterministic for future networking.
    /// </summary>
    public sealed class UnitMotor : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("Units per second movement speed.")]
        [SerializeField] private float moveSpeed = 2.0f;

        [Tooltip("Stop at this Y so units don't walk forever beyond the arena.")]
        [SerializeField] private float maxY = 10.0f;

        private void Update()
        {
            // Move upward every frame.
            transform.position += Vector3.up * (moveSpeed * Time.deltaTime);

            // Safety clamp for MVP.
            if (transform.position.y >= maxY)
            {
                // For MVP, just stop (later: attack tower, despawn, etc.)
                transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
                enabled = false;
            }
        }
    }
}
