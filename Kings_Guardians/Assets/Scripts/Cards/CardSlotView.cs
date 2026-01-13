using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KingGuardians.UI
{
    /// <summary>
    /// UI for one card slot (icon + cost + selection highlight).
    /// </summary>
    public sealed class CardSlotView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private GameObject selectedHighlight; // optional

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
