using UnityEngine;

namespace KingGuardians.Core
{
    /// <summary>
    /// Fits an orthographic camera to the battlefield defined by BattlefieldConfig.
    /// Ensures towers/lanes are visible in Game view on all aspect ratios.
    ///
    /// Portrait design:
    /// - Battlefield bounds are centered at (0,0)
    /// - Width = HalfArenaWidth * 2
    /// - Height = HalfArenaHeight * 2
    ///
    /// This script sets:
    /// - Camera position (centered)
    /// - Orthographic size (fits height + safe padding)
    /// - Optional: can also ensure width fits if needed on very narrow devices
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class BattleCameraFitter : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private BattlefieldConfig battlefieldConfig;

        [Header("Padding (world units)")]
        [Tooltip("Extra space around the battlefield so towers are not at the very edge.")]
        [Min(0f)][SerializeField] private float padding = 1.0f;

        [Header("Behavior")]
        [Tooltip("If true, centers camera to (0,0). Keep true for MVP.")]
        [SerializeField] private bool centerOnBattlefield = true;

        private Camera _cam;

        private void Awake()
        {
            _cam = GetComponent<Camera>();

            if (battlefieldConfig == null)
            {
                Debug.LogError("[BattleCameraFitter] BattlefieldConfig is missing. Assign BattlefieldConfig_MVP.", this);
                enabled = false;
                return;
            }

            if (!_cam.orthographic)
            {
                Debug.LogWarning("[BattleCameraFitter] Camera is not Orthographic. Switching to Orthographic for 2D.", this);
                _cam.orthographic = true;
            }

            Fit();
        }

        /// <summary>
        /// Fits camera to battlefield bounds.
        /// Call this after you change battlefield sizes or positions.
        /// </summary>
        [ContextMenu("Fit Now")]
        public void Fit()
        {
            // 1) Center camera (optional but recommended for MVP)
            if (centerOnBattlefield)
            {
                // Keep Z as-is (usually -10).
                transform.position = new Vector3(0f, 0f, transform.position.z);
            }

            // 2) Compute required ortho size to fit battlefield HEIGHT in view.
            // OrthographicSize = half of visible height in world units.
            float targetHalfHeight = battlefieldConfig.HalfArenaHeight + padding;

            // 3) Some devices are very narrow; height-fit might clip width.
            // If you want to guarantee width too, compute the needed half-height to fit width:
            // visibleHalfWidth = orthographicSize * aspect
            // To fit width: orthographicSize >= (requiredHalfWidth / aspect)
            float requiredHalfWidth = battlefieldConfig.HalfArenaWidth + padding;
            float sizeToFitWidth = requiredHalfWidth / Mathf.Max(0.01f, _cam.aspect);

            // Choose the larger size so BOTH width and height fit.
            _cam.orthographicSize = Mathf.Max(targetHalfHeight, sizeToFitWidth);

            // 4) Optional near/far clip sane defaults (helps avoid accidental clipping)
            _cam.nearClipPlane = -50f;
            _cam.farClipPlane = 50f;
        }
    }
}
