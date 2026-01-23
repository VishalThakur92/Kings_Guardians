using UnityEngine;
using UnityEngine.SceneManagement;

namespace KingGuardians.UI
{
    /// <summary>
    /// Controls the Main Menu.
    /// MVP responsibility:
    /// - Start the game on button press
    ///
    /// SOLID:
    /// - No gameplay logic
    /// - Only handles scene transition
    /// </summary>
    public sealed class MainMenuController : MonoBehaviour
    {
        [Header("Scene Names")]
        [Tooltip("Name of the battle scene to load.")]
        [SerializeField] private string battleSceneName = "Battle";

        /// <summary>
        /// Called by Start Game button.
        /// </summary>
        public void OnStartGamePressed()
        {
            SceneManager.LoadScene(battleSceneName);
        }
    }
}
