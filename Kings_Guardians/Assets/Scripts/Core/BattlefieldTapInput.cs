using UnityEngine;
using UnityEngine.EventSystems;

namespace KingGuardians.Core
{
    /// <summary>
    /// Converts touch/mouse to world and requests deployment.
    /// IMPORTANT:
    /// Ignores input when the pointer is over UI, otherwise clicking cards also triggers deployment.
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

            // --- Touch (mobile) ---
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);

                if (t.phase == TouchPhase.Began)
                {
                    // If finger started on UI, ignore.
                    if (IsPointerOverUI(t.fingerId))
                        return;

                    TryAtScreen(t.position);
                }
                return;
            }

#if UNITY_EDITOR
            // --- Mouse (editor) ---
            if (allowMouseInEditor && Input.GetMouseButtonDown(0))
            {
                // If clicking UI (card buttons), ignore.
                if (IsPointerOverUI())
                    return;

                TryAtScreen(Input.mousePosition);
            }
#endif
        }

        private void TryAtScreen(Vector2 screenPos)
        {
            Vector3 w = _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
            _controller.TryDeployAtWorld(new Vector2(w.x, w.y));
        }

        private bool IsPointerOverUI()
        {
            if (EventSystem.current == null) return false;
            return EventSystem.current.IsPointerOverGameObject();
        }

        private bool IsPointerOverUI(int fingerId)
        {
            if (EventSystem.current == null) return false;
            return EventSystem.current.IsPointerOverGameObject(fingerId);
        }
    }
}
