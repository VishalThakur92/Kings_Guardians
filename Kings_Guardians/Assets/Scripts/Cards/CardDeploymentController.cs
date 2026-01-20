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
        private readonly Combat.SpellValidator _spellValidator;
        private readonly Combat.SpellCaster _spellCaster;


        public CardDeploymentController(
            BattlefieldConfig cfg,
            DeploymentValidator validator,
            UnitSpawner spawner,
            EnergySystem energy,
            HandModel hand,
            Camera cam,
            Combat.SpellValidator spellValidator,
            Combat.SpellCaster spellCaster)
        {
            _cfg = cfg;
            _validator = validator;
            _spawner = spawner;
            _energy = energy;
            _hand = hand;
            _cam = cam;
            _spellValidator = spellValidator;
            _spellCaster = spellCaster;
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
            if (card == null) return false;

            // Must have enough energy for either type
            if (!_energy.CanSpend(card.EnergyCost)) return false;

            Vector3 w = _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
            Vector2 worldPos = new Vector2(w.x, w.y);

            // UNIT card preview
            if (card.Kind == KingGuardians.Cards.CardKind.Unit)
            {
                if (card.SpawnPrefab == null) return false;

                // Must be inside arena and inside player deploy zone
                if (!IsInsideArena(worldPos, _cfg)) return false;
                if (!_validator.IsPlayerDeployAllowed(worldPos)) return false;

                return true;
            }

            // SPELL card preview
            if (card.Kind == KingGuardians.Cards.CardKind.Spell)
            {
                if (card.Spell == null) return false;
                return _spellValidator.CanCastAt(card.Spell, worldPos);
            }

            return false;
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
            if (card == null) return false;

            Vector3 w = _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
            Vector2 worldPos = new Vector2(w.x, w.y);

            // Spend energy first only if it will succeed (so validate first).
            bool valid = CanDeployAtScreen(screenPos);
            if (!valid) return false;

            // Now spend (authoritative spend point for MVP)
            if (!_energy.TrySpend(card.EnergyCost)) return false;

            if (card.Kind == KingGuardians.Cards.CardKind.Unit)
            {
                var snapped = BattlefieldMath.SnapToNearestLane(worldPos, _cfg);

                var unitGo = _spawner.Spawn(card.SpawnPrefab, snapped, "P");
                // Apply stats (existing logic you already added)
                ApplyUnitStatsIfAny(unitGo, card);
            }
            else if (card.Kind == KingGuardians.Cards.CardKind.Spell)
            {
                _spellCaster.Cast(card.Spell, worldPos);
            }

            // Cycle card after successful action
            _hand.UseCardAt(SelectedSlot);
            return true;
        }

        private void ApplyUnitStatsIfAny(GameObject unitGo, KingGuardians.Cards.CardDefinition card)
        {
            if (unitGo == null || card.UnitStats == null) return;

            var hp = unitGo.GetComponent<KingGuardians.Units.UnitHealth>();
            if (hp != null) hp.ApplyMaxHp(card.UnitStats.MaxHp);

            var motor = unitGo.GetComponent<KingGuardians.Units.UnitMotor>();
            if (motor != null) motor.ApplyMoveSpeed(card.UnitStats.MoveSpeed);

            var atk = unitGo.GetComponent<KingGuardians.Units.UnitAttack>();
            if (atk != null) atk.ApplyAttackStats(card.UnitStats.DamagePerHit, card.UnitStats.AttackInterval);

            var desc = unitGo.GetComponent<KingGuardians.Units.UnitDescriptor>();
            if (desc == null) desc = unitGo.AddComponent<KingGuardians.Units.UnitDescriptor>();
            desc.Apply(card.UnitStats.Domain, card.UnitStats.CanTarget);
        }



    }
}
