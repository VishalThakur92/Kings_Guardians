using UnityEngine;

namespace KingGuardians.Core
{
    /// <summary>
    /// Reads player input and requests troop deployment.
    /// MVP: left-click (Editor) or tap (mobile) deploys a placeholder unit.
    /// </summary>
    public sealed class DeploymentInput : MonoBehaviour
    {
        [Header("Input")]
        [Tooltip("If true, allows mouse click in editor as a tap.")]
        [SerializeField] private bool allowMouseInEditor = true;

        [Tooltip("If true, clamp Y into player deploy zone even if tapped slightly above boundary.")]
        [SerializeField] private bool clampIntoDeployZone = true;

        private BattlefieldConfig _cfg;
        private DeploymentValidator _validator;
        private UnitSpawner _spawner;

        private Camera _cam;

        /// <summary>
        /// Called by DeploymentInstaller. Keeps dependencies explicit.
        /// </summary>
        public void Initialize(BattlefieldConfig cfg, DeploymentValidator validator, UnitSpawner spawner)
        {
            _cfg = cfg;
            _validator = validator;
            _spawner = spawner;
            _cam = Camera.main;

            if (_cam == null)
            {
                Debug.LogError("[DeploymentInput] No Camera.main found. Ensure Main Camera is tagged MainCamera.", this);
                enabled = false;
            }
        }

        private void Update()
        {
            if (_cfg == null || _validator == null || _spawner == null) return;

            // Mobile touch
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began)
                {
                    TryDeployAtScreenPoint(t.position);
                }
                return;
            }

#if UNITY_EDITOR
            // Editor mouse click
            if (allowMouseInEditor && Input.GetMouseButtonDown(0))
            {
                TryDeployAtScreenPoint(Input.mousePosition);
            }
#endif
        }

        private void TryDeployAtScreenPoint(Vector2 screenPos)
        {
            Vector2 world = ScreenToWorld2D(screenPos);

            // Clamp to arena first (prevents weird out-of-bounds spawns).
            world = BattlefieldMath.ClampToArena(world, _cfg);

            // Optionally clamp Y into player zone (makes MVP feel forgiving).
            if (clampIntoDeployZone)
            {
                world.y = _validator.ClampToPlayerDeployY(world.y);
            }

            // Validate deploy rules.
            if (!_validator.IsPlayerDeployAllowed(world))
            {
                // For MVP, just ignore.
                // Later you can show UI feedback: "Can't deploy here".
                return;
            }

            // Snap onto nearest lane.
            Vector2 snapped = BattlefieldMath.SnapToNearestLane(world, _cfg);

            // Spawn unit.
            _spawner.Spawn(snapped, "P");
        }

        private Vector2 ScreenToWorld2D(Vector2 screenPos)
        {
            // For 2D orthographic camera, Z does not matter as long as we set it to camera plane distance.
            Vector3 s = new Vector3(screenPos.x, screenPos.y, 0f);
            Vector3 w = _cam.ScreenToWorldPoint(s);
            return new Vector2(w.x, w.y);
        }
    }
}
