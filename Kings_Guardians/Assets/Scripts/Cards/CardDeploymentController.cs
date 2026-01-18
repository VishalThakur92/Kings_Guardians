using UnityEngine;
using KingGuardians.Cards;

namespace KingGuardians.Core
{
    /// <summary>
    /// Core deploy logic. UI calls into this via ICardDeployController.
    /// </summary>
    public sealed class CardDeploymentController : ICardDeployController
    {
        public int SelectedSlot { get; private set; } = -1;

        private readonly BattlefieldConfig _cfg;
        private readonly DeploymentValidator _validator;
        private readonly UnitSpawner _spawner;
        private readonly EnergySystem _energy;
        private readonly HandModel _hand;
        private readonly Camera _cam;

        public CardDeploymentController(
            BattlefieldConfig cfg,
            DeploymentValidator validator,
            UnitSpawner spawner,
            EnergySystem energy,
            HandModel hand,
            Camera cam)
        {
            _cfg = cfg;
            _validator = validator;
            _spawner = spawner;
            _energy = energy;
            _hand = hand;
            _cam = cam;
        }

        public void SelectSlot(int slotIndex) => SelectedSlot = slotIndex;

        /// <summary>
        /// PREVIEW ONLY:
        /// Returns whether the currently selected card could be deployed at this screen position
        /// using current rules (zone + energy + prefab validity).
        /// No energy is spent and nothing is spawned.
        /// </summary>
        public bool CanDeployAtScreen(Vector2 screenPos)
        {
            if (SelectedSlot < 0 || _cam == null) return false;

            var card = _hand.GetCardAt(SelectedSlot);
            if (card == null || card.SpawnPrefab == null) return false;

            // Energy check (no spend)
            if (!_energy.CanSpend(card.EnergyCost)) return false;

            Vector3 w = _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
            Vector2 worldPos = new Vector2(w.x, w.y);

            // IMPORTANT: For preview, do NOT clamp into deploy zone.
            // Validate the raw position so the ghost turns red outside the spawnable area.
            if (!IsInsideArena(worldPos, _cfg)) return false;
            if (!_validator.IsPlayerDeployAllowed(worldPos)) return false;

            return true;
        }
        private static bool IsInsideArena(Vector2 p, BattlefieldConfig cfg)
        {
            return p.x >= -cfg.HalfArenaWidth && p.x <= cfg.HalfArenaWidth &&
                   p.y >= -cfg.HalfArenaHeight && p.y <= cfg.HalfArenaHeight;
        }

        /// <summary>
        /// FINAL DEPLOY:
        /// Attempts to deploy selected card at screen position.
        /// Spends energy, spawns prefab, cycles hand.
        /// </summary>
        public bool TryDeployAtScreen(Vector2 screenPos)
        {
            if (SelectedSlot < 0 || _cam == null) return false;

            var card = _hand.GetCardAt(SelectedSlot);
            if (card == null || card.SpawnPrefab == null) return false;

            Vector3 w = _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
            Vector2 worldPos = new Vector2(w.x, w.y);

            // Validate RAW drop position first (must actually be in the spawnable area)
            if (!IsInsideArena(worldPos, _cfg)) return false;
            if (!_validator.IsPlayerDeployAllowed(worldPos)) return false;

            // Now it's safe to clamp within arena (safety) and snap to lane.
            worldPos = BattlefieldMath.ClampToArena(worldPos, _cfg);
            var snapped = BattlefieldMath.SnapToNearestLane(worldPos, _cfg);

            if (!_energy.TrySpend(card.EnergyCost))
                return false;

            var unitGo = _spawner.Spawn(card.SpawnPrefab, snapped, "P");

            _hand.UseCardAt(SelectedSlot);

            // Apply stats if this is a unit card
            if (unitGo != null && card.UnitStats != null)
            {
                // Health
                var hp = unitGo.GetComponent<KingGuardians.Units.UnitHealth>();
                if (hp != null) hp.ApplyMaxHp(card.UnitStats.MaxHp);

                // Movement
                var motor = unitGo.GetComponent<KingGuardians.Units.UnitMotor>();
                if (motor != null) motor.ApplyMoveSpeed(card.UnitStats.MoveSpeed);

                // Attack
                var atk = unitGo.GetComponent<KingGuardians.Units.UnitAttack>();
                if (atk != null) atk.ApplyAttackStats(card.UnitStats.DamagePerHit, card.UnitStats.AttackInterval);

                //Domain
                var desc = unitGo.GetComponent<KingGuardians.Units.UnitDescriptor>();
                if (desc == null) desc = unitGo.AddComponent<KingGuardians.Units.UnitDescriptor>();
                desc.Apply(card.UnitStats.Domain, card.UnitStats.CanTarget);

            }

            return true;
        }


    }
}
