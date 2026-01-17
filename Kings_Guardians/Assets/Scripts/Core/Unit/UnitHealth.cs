using System;
using UnityEngine;
using KingGuardians.Combat;

namespace KingGuardians.Units
{
    /// <summary>
    /// MVP unit health.
    /// Even if units aren't being damaged yet, adding this now enables UI bars
    /// and prepares for tower attacks later.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UnitHealth : MonoBehaviour, IDamageable, IHealthReadable
    {
        [Header("Stats (MVP)")]
        [SerializeField] private int maxHp = 100;

        private int _currentHp;

        public bool IsAlive => _currentHp > 0;

        public int CurrentHp => _currentHp;
        public int MaxHp => maxHp;

        public event Action<int, int> OnHealthChanged;

        private void Awake()
        {
            _currentHp = maxHp;
            OnHealthChanged?.Invoke(_currentHp, maxHp);
        }

        /// <summary>
        /// Optional init for later (spawner can set based on troop data).
        /// </summary>
        public void Init(int hp)
        {
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

        /// <summary>
        /// Applies stats to this unit. Safe to call right after spawn.
        /// </summary>
        public void ApplyMaxHp(int hp)
        {
            maxHp = Mathf.Max(1, hp);
            _currentHp = maxHp;
            OnHealthChanged?.Invoke(_currentHp, maxHp);
        }


        private void Die()
        {
            // MVP: destroy unit (later: death anim, pooling)
            Destroy(gameObject);
        }
    }
}
