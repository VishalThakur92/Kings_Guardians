using System;
using UnityEngine;
using KingGuardians.Combat;
using KingGuardians.Core;

namespace KingGuardians.Towers
{
    /// <summary>
    /// MVP tower HP system.
    /// - Implements IDamageable for combat
    /// - Implements IHealthReadable for UI
    /// - Disables the tower GameObject on death (MVP)
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TowerHealth : MonoBehaviour, IDamageable, IHealthReadable
    {
        [Header("Identity")]
        [SerializeField] private TeamId team = TeamId.Enemy;
        [SerializeField] private TowerType towerType = TowerType.Outpost;

        [Header("Stats (MVP)")]
        [SerializeField] private int maxHp = 200;

        private int _currentHp;

        public bool IsAlive => _currentHp > 0;

        public TeamId Team => team;
        public TowerType Type => towerType;

        public int CurrentHp => _currentHp;
        public int MaxHp => maxHp;

        public event Action<int, int> OnHealthChanged;

        private void Awake()
        {
            _currentHp = maxHp;
            OnHealthChanged?.Invoke(_currentHp, maxHp);
        }

        /// <summary>
        /// Runtime init called by TowerSpawner.
        /// </summary>
        public void Init(TeamId newTeam, TowerType newType, int hp)
        {
            team = newTeam;
            towerType = newType;

            maxHp = Mathf.Max(1, hp);
            _currentHp = maxHp;

            OnHealthChanged?.Invoke(_currentHp, maxHp);
        }

        public void TakeDamage(int amount)
        {
            if (!IsAlive) return;

            int dmg = Mathf.Max(0, amount);
            if (dmg == 0) return;

            _currentHp = Mathf.Max(0, _currentHp - dmg);
            OnHealthChanged?.Invoke(_currentHp, maxHp);

            if (_currentHp == 0)
                Die();
        }

        private void Die()
        {
            // MVP behavior: disable object (later: VFX, animation, events)
            gameObject.SetActive(false);
        }
    }
}
