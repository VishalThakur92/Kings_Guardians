using UnityEngine;

namespace KingGuardians.Units
{
    /// <summary>
    /// MVP movement component:
    /// - Moves unit along a direction (portrait battlefield).
    /// - Player units move upward (+Y), enemy units move downward (-Y).
    /// - Can be stopped/resumed by targeting/combat.
    /// </summary>
    public sealed class UnitMotor : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2.0f;

        [Tooltip("Movement direction in world space. For portrait lanes use (0, +1) or (0, -1).")]
        [SerializeField] private Vector2 moveDirection = Vector2.up;

        private bool _isStopped;

        public void Stop() => _isStopped = true;
        public void Resume() => _isStopped = false;

        /// <summary>
        /// Apply movement speed from stats (used by card/unit definitions).
        /// </summary>
        public void ApplyMoveSpeed(float speed) => moveSpeed = Mathf.Max(0f, speed);

        /// <summary>
        /// Sets movement direction (Player: up, Enemy: down).
        /// </summary>
        public void SetDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.0001f) return;
            moveDirection = direction.normalized;
        }

        private void Update()
        {
            if (_isStopped) return;

            // Deterministic movement step.
            Vector3 delta = new Vector3(moveDirection.x, moveDirection.y, 0f) * (moveSpeed * Time.deltaTime);
            transform.position += delta;
        }
    }
}
