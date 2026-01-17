using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using KingGuardians.Core;
using KingGuardians.Cards;

namespace KingGuardians.UI
{
    /// <summary>
    /// Enables drag-to-deploy for a card UI slot.
    ///
    /// Key behavior:
    /// 1) User drags the card -> we show a "ghost" image that follows the pointer.
    /// 2) On release -> we ask the deploy controller to deploy at the release screen position.
    ///
    /// IMPORTANT implementation detail:
    /// The ghost must be positioned in the SAME coordinate space as the Canvas used for the UI.
    /// If the ghost stays parented under the card slot (which may be nested under other rects),
    /// converting screen -> canvas local coordinates will produce an offset.
    ///
    /// Fix:
    /// - OnBeginDrag: temporarily re-parent the ghost under the ROOT CANVAS.
    /// - While dragging: convert screen position to root-canvas local position and set anchoredPosition.
    /// - OnEndDrag: restore ghost to its original parent and anchored position.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class CardDragDeploy : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Slot")]
        [Tooltip("Hand index this UI represents (0..3).")]
        [SerializeField] private int slotIndex = 0;

        [Header("Ghost UI")]
        [Tooltip("The Image used as the dragging ghost (usually a dedicated 'DragGhost' child Image).")]
        [SerializeField] private Image ghostImage;

        // Cached references for coordinate conversion.
        private Canvas _rootCanvas;
        private RectTransform _canvasRect;
        private RectTransform _ghostRect;

        // We restore the ghost to its original hierarchy after drag.
        private Transform _ghostOriginalParent;
        private Vector2 _ghostOriginalAnchoredPos;

        // CanvasGroup is used to disable raycast blocking while dragging.
        private CanvasGroup _canvasGroup;

        // Dependencies injected by the installer (keeps UI decoupled from gameplay logic).
        private ICardDeployController _deployController;
        private ICardProvider _cardProvider;

        [Header("Ghost Size")]
        [Tooltip("Scale multiplier applied to the ghost while dragging (1 = same as card icon).")]
        [SerializeField] private float ghostScale = 0.75f;

        [Header("Ghost Visuals")]
        [Tooltip("Color used when placement is valid.")]
        [SerializeField] private Color validTint = Color.white;

        [Tooltip("Color used when placement is NOT valid.")]
        [SerializeField] private Color invalidTint = new Color(1f, 0.2f, 0.2f, 1f);
        private Color _ghostBaseColor = Color.white;



        /// <summary>
        /// Injects dependencies. Call from your installer after scene loads.
        /// </summary>
        public void Initialize(ICardDeployController deployController, ICardProvider cardProvider, Canvas rootCanvas)
        {
            _deployController = deployController;
            _cardProvider = cardProvider;
            _rootCanvas = rootCanvas;

            // Root canvas rect is used to convert screen coordinates into canvas local coordinates.
            _canvasRect = _rootCanvas != null ? _rootCanvas.transform as RectTransform : null;
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            // Cache the ghost RectTransform for fast updates.
            if (ghostImage != null)
            {
                _ghostRect = ghostImage.rectTransform;

                // Cache original color so we can restore it when valid
                _ghostBaseColor = ghostImage.color;
            }

            // Ghost should be hidden by default.
            SetGhostVisible(false);
        }

        /// <summary>
        /// Called when user starts dragging this card.
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            // Validate everything needed to show and move the ghost correctly.
            if (_deployController == null || _cardProvider == null || _rootCanvas == null || _ghostRect == null)
                return;

            // Mark this slot as selected so the deploy controller uses the correct card.
            _deployController.SelectSlot(slotIndex);

            // While dragging, the card UI should not block raycasts. This prevents UI from interfering with drag flow.
            _canvasGroup.blocksRaycasts = false;

            // Set ghost sprite to match the currently displayed card.
            CardDefinition card = _cardProvider.GetCardAt(slotIndex);
            if (card != null && ghostImage != null)
                ghostImage.sprite = card.Icon;

            // --- CRITICAL FIX FOR OFFSET ---
            // Re-parent the ghost to the root canvas so its anchoredPosition is interpreted in canvas space.
            // If we keep it under a nested RectTransform (card slot), the local coordinates won't match.
            _ghostOriginalParent = _ghostRect.parent;
            _ghostOriginalAnchoredPos = _ghostRect.anchoredPosition;

            // Ensure the ghost uses centered anchors so anchoredPosition maps cleanly to canvas local coordinates.
            // Stretch anchors can introduce offsets because anchoredPosition is then relative to a stretched rect.
            _ghostRect.anchorMin = new Vector2(0.5f, 0.5f);
            _ghostRect.anchorMax = new Vector2(0.5f, 0.5f);
            _ghostRect.pivot = new Vector2(0.5f, 0.5f);

            _ghostRect.SetParent(_rootCanvas.transform, worldPositionStays: false);
            _ghostRect.SetAsLastSibling(); // Render on top of other UI.
                                           // Explicitly control ghost size so it looks consistent across resolutions.
            _ghostRect.localScale = Vector3.one * ghostScale;

            SetGhostVisible(true);


            // Position ghost immediately on drag start.
            UpdateGhostPosition(eventData);


            // Update tint immediately at drag start
            UpdateGhostTint(eventData.position);
        }

        /// <summary>
        /// Called every frame during drag; keeps ghost following the pointer/finger.
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (_deployController == null) return;
            UpdateGhostTint(eventData.position);
            UpdateGhostPosition(eventData);

        }

        /// <summary>
        /// Called when user releases the drag.
        /// Attempts deployment at the release position.
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (_deployController == null) return;

            // Restore card raycast blocking.
            _canvasGroup.blocksRaycasts = true;

            // Restore ghost back into its original hierarchy and position.
            if (_ghostRect != null && _ghostOriginalParent != null)
            {
                _ghostRect.SetParent(_ghostOriginalParent, worldPositionStays: false);
                _ghostRect.anchoredPosition = _ghostOriginalAnchoredPos;
            }

            SetGhostVisible(false);

            // Restore scale so it doesn't affect layout when returned to slot.
            _ghostRect.localScale = Vector3.one;

            // Restore ghost color so it doesn't remain tinted next drag
            if (ghostImage != null)
                ghostImage.color = _ghostBaseColor;


            // Request deployment at the release screen position.
            // Controller will validate: deploy zone + energy + lane snap.
            _deployController.TryDeployAtScreen(eventData.position);
        }


        /// <summary>
        /// Updates ghost tint based on whether the selected card can be deployed at the current screen position.
        /// This is preview-only and does NOT spend energy.
        /// </summary>
        private void UpdateGhostTint(Vector2 screenPos)
        {
            if (ghostImage == null || _deployController == null) return;

            bool canPlace = _deployController.CanDeployAtScreen(screenPos);


            // If valid, keep normal appearance. If invalid, tint red.
            // Use base color so designers can set alpha or styling in prefab.
            ghostImage.color = canPlace ? (_ghostBaseColor * validTint) : invalidTint;
        }


        private void UpdateGhostPosition(PointerEventData eventData)
        {
            if (_ghostRect == null || _canvasRect == null || _rootCanvas == null) return;

            // IMPORTANT:
            // ScreenPointToLocalPointInRectangle must use the SAME camera that the Canvas uses
            // (or null for Screen Space - Overlay).
            //
            // eventData.pressEventCamera can be:
            // - null even when using Screen Space - Camera (depending on event source)
            // - a different camera than the Canvas camera
            //
            // Using Canvas camera removes subtle offsets.
            Camera camForCanvas = null;

            switch (_rootCanvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    // Overlay canvases use no camera for coordinate conversion.
                    camForCanvas = null;
                    break;

                case RenderMode.ScreenSpaceCamera:
                case RenderMode.WorldSpace:
                    // For Camera/World canvases, use the canvas' worldCamera if available.
                    // If it's missing, fallback to Camera.main.
                    camForCanvas = _rootCanvas.worldCamera != null ? _rootCanvas.worldCamera : Camera.main;
                    break;
            }

            // Convert screen coordinates (pixels) into local canvas coordinates.
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvasRect,
                    eventData.position,
                    camForCanvas,
                    out Vector2 localPos))
            {
                _ghostRect.anchoredPosition = localPos;
            }
        }


        /// <summary>
        /// Shows or hides the ghost image GameObject.
        /// </summary>
        private void SetGhostVisible(bool visible)
        {
            if (ghostImage != null)
                ghostImage.gameObject.SetActive(visible);
        }
    }
}
