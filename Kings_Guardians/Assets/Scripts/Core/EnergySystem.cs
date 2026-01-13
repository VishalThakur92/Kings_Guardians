using System;
using UnityEngine;

namespace KingGuardians.Core
{
    /// <summary>
    /// Runtime energy model: regen + spend.
    /// UI listens via OnChanged (event-driven).
    /// </summary>
    public sealed class EnergySystem
    {
        public event Action<int, int> OnChanged; // (current, max)

        private readonly EnergyConfig _cfg;
        private float _accumulator;

        public int Current { get; private set; }
        public int Max => _cfg.MaxEnergy;

        public EnergySystem(EnergyConfig cfg)
        {
            _cfg = cfg;
            Current = Mathf.Clamp(cfg.StartEnergy, 0, cfg.MaxEnergy);
            OnChanged?.Invoke(Current, Max);
        }

        public void Tick(float deltaTime)
        {
            // Accumulate fractional regen and convert to integer energy.
            _accumulator += _cfg.RegenPerSecond * deltaTime;

            if (_accumulator < 1f) return;

            int gained = Mathf.FloorToInt(_accumulator);
            _accumulator -= gained;

            SetCurrent(Current + gained);
        }

        public bool CanSpend(int amount) => amount <= Current;

        public bool TrySpend(int amount)
        {
            if (amount <= 0) return true;
            if (!CanSpend(amount)) return false;

            SetCurrent(Current - amount);
            return true;
        }

        private void SetCurrent(int value)
        {
            int clamped = Mathf.Clamp(value, 0, _cfg.MaxEnergy);
            if (clamped == Current) return;

            Current = clamped;
            OnChanged?.Invoke(Current, Max);
        }
    }
}
