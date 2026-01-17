using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KingGuardians.UI
{
    /// <summary>
    /// Minimal end-of-match UI:
    /// - Shows winner text
    /// - Offers Restart button
    /// </summary>
    public sealed class MatchUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private Button restartButton;

        private void Awake()
        {
            // Defensive: keep UI hidden at scene start.
            if (resultPanel != null)
                resultPanel.SetActive(false);
        }

        /// <summary>
        /// Shows the match result popup.
        /// </summary>
        public void ShowResult(string message)
        {
            if (resultText != null)
                resultText.text = message;

            if (resultPanel != null)
                resultPanel.SetActive(true);
        }

        /// <summary>
        /// Hides the match result popup.
        /// </summary>
        public void HideResult()
        {
            if (resultPanel != null)
                resultPanel.SetActive(false);
        }

        public void SetRestartAction(System.Action onRestart)
        {
            if (restartButton == null) return;

            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() => onRestart?.Invoke());
        }
    }
}
