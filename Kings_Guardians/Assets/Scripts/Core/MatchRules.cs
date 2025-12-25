using UnityEngine;
using KingGuardians.Towers;

namespace KingGuardians.Core
{
    /// <summary>
    /// MVP match rule checker:
    /// - Watches main towers
    /// - Declares winner when one is destroyed
    /// </summary>
    public sealed class MatchRules : MonoBehaviour
    {
        [SerializeField] private string playerMainTowerName = "P_MainTower";
        [SerializeField] private string enemyMainTowerName = "E_MainTower";

        private TowerHealth _playerMain;
        private TowerHealth _enemyMain;

        private bool _matchEnded;

        private void Start()
        {
            // MVP simple lookup by name; later you will store references at spawn time.
            var p = GameObject.Find(playerMainTowerName);
            var e = GameObject.Find(enemyMainTowerName);

            if (p != null) _playerMain = p.GetComponent<TowerHealth>();
            if (e != null) _enemyMain = e.GetComponent<TowerHealth>();

            if (_playerMain == null || _enemyMain == null)
            {
                Debug.LogError("[MatchRules] Main tower references not found. Check tower names/spawn.", this);
                enabled = false;
            }
        }

        private void Update()
        {
            if (_matchEnded) return;

            if (_enemyMain != null && !_enemyMain.IsAlive)
            {
                EndMatch("PLAYER WINS");
            }
            else if (_playerMain != null && !_playerMain.IsAlive)
            {
                EndMatch("ENEMY WINS");
            }
        }

        private void EndMatch(string result)
        {
            _matchEnded = true;
            Debug.Log($"[MatchRules] Match Ended: {result}");

            // MVP: Pause gameplay. Later: show UI result screen.
            Time.timeScale = 0f;
        }
    }
}
