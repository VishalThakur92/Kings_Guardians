using UnityEngine;

namespace KingGuardians.VFX
{
    /// <summary>
    /// Minimal, radius-accurate spell VFX.
    ///
    /// RULES:
    /// - Visual size MUST exactly match spell damage radius.
    /// - No scale animation, no exaggeration.
    /// - Only fades out over time.
    ///
    /// Assumption:
    /// - The sprite used is authored at ~1 world unit size.
    ///   (Adjust prefab scale ONCE if needed; do not compensate in code.)
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class OneShotSpellVfx : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Lifetime")]
        [Tooltip("How long the VFX stays visible (seconds).")]
        [Min(0.05f)]
        [SerializeField] private float duration = 0.35f;

        private float _time;
        private Color _baseColor;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (spriteRenderer != null)
                _baseColor = spriteRenderer.color;
        }

        /// <summary>
        /// Initializes the VFX size so that:
        /// diameter in world units == spell.Radius * 2
        /// </summary>
        public void InitRadius(float radius)
        {
            // World-space diameter
            float diameter = Mathf.Max(0.01f, radius * 2f);

            // IMPORTANT:
            // We apply the scale ONCE and never touch it again.
            transform.localScale = Vector3.one * diameter;
        }

        private void Update()
        {
            _time += Time.deltaTime;

            float t = Mathf.Clamp01(_time / duration);

            // Fade alpha only — no scaling, no movement
            if (spriteRenderer != null)
            {
                Color c = _baseColor;
                c.a = Mathf.Lerp(_baseColor.a, 0f, t);
                spriteRenderer.color = c;
            }

            if (_time >= duration)
                Destroy(gameObject);
        }
    }
}
