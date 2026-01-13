using UnityEngine;

namespace KingGuardians.Core
{
    /// <summary>
    /// Converts touch/mouse to world and requests deployment.
    /// </summary>
    public sealed class BattlefieldTapInput : MonoBehaviour
    {
        [SerializeField] private bool allowMouseInEditor = true;

        private Camera _cam;
        private CardDeploymentController _controller;

        public void Initialize(CardDeploymentController controller)
        {
            _controller = controller;
            _cam = Camera.main;

            if (_cam == null)
            {
                Debug.LogError("[BattlefieldTapInput] Missing Camera.main (tag MainCamera).", this);
                enabled = false;
            }
        }

        private void Update()
        {
            if (_controller == null) return;

            // Touch
            if (Input.touchCount > 0)
            {
                var t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began)
                    TryAtScreen(t.position);
                return;
            }

#if UNITY_EDITOR
            if (allowMouseInEditor && Input.GetMouseButtonDown(0))
                TryAtScreen(Input.mousePosition);
#endif
        }

        private void TryAtScreen(Vector2 screenPos)
        {
            Vector3 w = _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
            _controller.TryDeployAtWorld(new Vector2(w.x, w.y));
        }
    }
}
