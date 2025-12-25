using UnityEngine;
using UnityEngine.UI;
using KingGuardians.Combat;

namespace KingGuardians.UI
{
    /// <summary>
    /// World-space health bar that binds to an IHealthReadable source.
    /// - Updates fill amount when health changes (event-driven)
    /// - Optionally billboard to camera for readability
    /// </summary>
    public sealed class WorldHealthBar : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image fillImage;

        [Header("Behavior")]
        [SerializeField] private bool faceCamera = true;

        private IHealthReadable _healthSource;
        private Camera _cam;

        private void Awake()
        {
            _cam = Camera.main;
        }

        /// <summary>
        /// Bind this bar to a health source. Safe to call once at spawn.
        /// </summary>
        public void Bind(IHealthReadable source)
        {
            Unbind();

            _healthSource = source;
            if (_healthSource == null) return;

            _healthSource.OnHealthChanged += HandleHealthChanged;

            // Initialize immediately
            HandleHealthChanged(_healthSource.CurrentHp, _healthSource.MaxHp);
        }

        private void OnDestroy()
        {
            Unbind();
        }

        private void Unbind()
        {
            if (_healthSource != null)
            {
                _healthSource.OnHealthChanged -= HandleHealthChanged;
                _healthSource = null;
            }
        }

        private void HandleHealthChanged(int current, int max)
        {
            if (fillImage == null) return;

            float pct = (max <= 0) ? 0f : Mathf.Clamp01((float)current / max);
            fillImage.fillAmount = pct;

            // Optional: hide bar when full HP (cleaner look)
            // gameObject.SetActive(pct < 1f);
        }

        private void LateUpdate()
        {
            if (!faceCamera) return;
            if (_cam == null) _cam = Camera.main;
            if (_cam == null) return;

            // Keep the bar facing the camera in 2D world-space canvas.
            transform.rotation = Quaternion.identity;
        }
    }
}
