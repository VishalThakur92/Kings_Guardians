using UnityEngine;

namespace KingGuardians.VFX
{
    /// <summary>
    /// Minimal one-shot spell VFX:
    /// - Scales a SpriteRenderer to match the spell radius (diameter = radius * 2)
    /// - Fades out over duration
    /// - Destroys itself at the end
    ///
    /// This keeps VFX independent from spell logic (SOLID).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class OneShotSpellVfx : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Timing")]
        [Tooltip("How long the VFX stays visible (seconds).")]
        [Min(0.05f)][SerializeField] private float duration = 0.35f;

        [Tooltip("Optional: small scale pop at start.")]
        [Min(0f)][SerializeField] private float startScaleMultiplier = 0.9f;

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
        /// Initializes the VFX size based on spell radius (world units).
        /// Should be called immediately after instantiate.
        /// </summary>
        public void InitRadius(float radius)
        {
            // Diameter in world units
            float diameter = Mathf.Max(0.01f, radius * 2f);

            // We assume the sprite is a unit circle-ish sprite (approx 1 world unit when scale = 1).
            // If your sprite pixels-per-unit differs, adjust prefab scale once and keep code unchanged.
            transform.localScale = Vector3.one * (diameter * startScaleMultiplier);
        }

        private void Update()
        {
            _time += Time.deltaTime;

            // Normalized progress 0..1
            float t = Mathf.Clamp01(_time / duration);

            // Fade alpha from 1 -> 0
            if (spriteRenderer != null)
            {
                var c = _baseColor;
                c.a = Mathf.Lerp(_baseColor.a, 0f, t);
                spriteRenderer.color = c;
            }

            // Slight scale-up as it fades (optional, gives "impact ring" feel)
            float scaleUp = Mathf.Lerp(1f, 1.1f, t);
            transform.localScale *= scaleUp;

            if (_time >= duration)
                Destroy(gameObject);
        }
    }
}
