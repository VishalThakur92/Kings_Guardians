using UnityEngine;
using UnityEngine.SceneManagement;
using KingGuardians.Towers;
using KingGuardians.UI;

namespace KingGuardians.Core
{
    /// <summary>
    /// Minimal match state controller:
    /// - Starts match immediately
    /// - Watches main towers
    /// - Declares win/lose
    /// - Freezes time and shows UI
    /// - Restarts scene on request
    ///
    /// Keeps logic simple for MVP; later you can add states and countdowns.
    /// </summary>
    public sealed class MatchController : MonoBehaviour
    {
        [Header("Tower Names (must match spawned names)")]
        [SerializeField] private string playerMainTowerName = "P_MainTower";
        [SerializeField] private string enemyMainTowerName = "E_MainTower";

        [Header("UI")]
        [SerializeField] private MatchUI matchUI;

        private TowerHealth _playerMain;
        private TowerHealth _enemyMain;

        private bool _ended;

        private void Start()
        {
            // Ensure time is running (in case previous match ended and you hit Play again in editor).
            Time.timeScale = 1f;

            // Cache main tower references by name (MVP simple).
            _playerMain = FindTowerHealth(playerMainTowerName);
            _enemyMain = FindTowerHealth(enemyMainTowerName);

            if (_playerMain == null || _enemyMain == null)
            {
                Debug.LogError("[MatchController] Main towers not found. Check TowerSpawner names.", this);
                enabled = false;
                return;
            }

            if (matchUI != null)
            {
                matchUI.HideResult();
                matchUI.SetRestartAction(RestartMatch);
            }
        }

        private void Update()
        {
            if (_ended) return;

            // Win/Lose detection
            if (!_enemyMain.IsAlive)
            {
                EndMatch("YOU WIN");
            }
            else if (!_playerMain.IsAlive)
            {
                EndMatch("YOU LOSE");
            }
        }

        private void EndMatch(string message)
        {
            _ended = true;

            // Freeze gameplay for MVP. (UI still works.)
            Time.timeScale = 0f;

            if (matchUI != null)
                matchUI.ShowResult(message);

            Debug.Log($"[MatchController] Match Ended: {message}");
        }

        private void RestartMatch()
        {
            // Restore time before reload.
            Time.timeScale = 1f;

            // Reload current scene (fastest, cleanest reset for MVP).
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private TowerHealth FindTowerHealth(string goName)
        {
            var go = GameObject.Find(goName);
            if (go == null) return null;
            return go.GetComponent<TowerHealth>();
        }
    }
}
