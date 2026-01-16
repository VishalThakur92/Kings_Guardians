using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KingGuardians.UI
{
    /// <summary>
    /// UI for one card slot (icon + cost + selection highlight).
    /// SlotIndex explicitly maps this UI element to a hand index (0..HandSize-1),
    /// so inspector array order cannot cause wrong spawns.
    /// </summary>
    public sealed class CardSlotView : MonoBehaviour
    {
        [Header("Slot")]
        [Tooltip("Hand index this UI represents (0..3).")]
        [SerializeField] private int slotIndex = 0;

        public int SlotIndex => slotIndex;

        [Header("UI")]
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private GameObject selectedHighlight;

        public Button Button => button;

        public void SetIcon(Sprite sprite)
        {
            if (icon != null) icon.sprite = sprite;
        }

        public void SetCost(int cost)
        {
            if (costText != null) costText.text = cost.ToString();
        }

        public void SetSelected(bool selected)
        {
            if (selectedHighlight != null) selectedHighlight.SetActive(selected);
        }

        public void SetInteractable(bool interactable)
        {
            if (button != null) button.interactable = interactable;
        }
    }
}
