using UnityEngine;
using KingGuardians.Cards;

namespace KingGuardians.Core
{
    /// <summary>
    /// Coordinates selected card + battlefield tap deployment.
    /// Owns no UI; UI calls SelectSlot().
    /// </summary>
    public sealed class CardDeploymentController
    {
        public int SelectedSlot { get; private set; } = -1;

        private readonly BattlefieldConfig _cfg;
        private readonly DeploymentValidator _validator;
        private readonly UnitSpawner _spawner;
        private readonly EnergySystem _energy;
        private readonly HandModel _hand;

        public CardDeploymentController(
            BattlefieldConfig cfg,
            DeploymentValidator validator,
            UnitSpawner spawner,
            EnergySystem energy,
            HandModel hand)
        {
            _cfg = cfg;
            _validator = validator;
            _spawner = spawner;
            _energy = energy;
            _hand = hand;
        }

        public void SelectSlot(int slotIndex)
        {
            SelectedSlot = slotIndex;
        }

        public bool TryDeployAtWorld(Vector2 worldPos)
        {
            if (SelectedSlot < 0) return false;

            // Clamp to arena
            worldPos = BattlefieldMath.ClampToArena(worldPos, _cfg);

            // Clamp into deploy zone for forgiving UX
            worldPos.y = _validator.ClampToPlayerDeployY(worldPos.y);

            if (!_validator.IsPlayerDeployAllowed(worldPos))
                return false;

            // Snap to nearest lane
            var snapped = BattlefieldMath.SnapToNearestLane(worldPos, _cfg);

            // Read selected card
            var card = _hand.GetCardAt(SelectedSlot);
            if (card == null || card.SpawnPrefab == null) return false;

            // Energy check
            if (!_energy.TrySpend(card.EnergyCost))
                return false;

            // Spawn
            _spawner.Spawn(card.SpawnPrefab, snapped, "P");


            // Cycle card
            _hand.UseCardAt(SelectedSlot);

            return true;
        }
    }
}
