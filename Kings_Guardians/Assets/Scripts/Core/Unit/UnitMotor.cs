using UnityEngine;

namespace KingGuardians.Units
{
    /// <summary>
    /// MVP movement component:
    /// - Moves unit forward along +Y (portrait battlefield).
    /// - Can be stopped/resumed by other systems (targeting/combat later).
    /// </summary>
    public sealed class UnitMotor : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2.0f;

        private bool _isStopped;

        public void Stop() => _isStopped = true;
        public void Resume() => _isStopped = false;

        private void Update()
        {
            if (_isStopped) return;

            transform.position += Vector3.up * (moveSpeed * Time.deltaTime);
        }
    }
}
