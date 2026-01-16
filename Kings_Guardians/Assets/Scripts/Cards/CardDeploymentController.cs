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
        /// Called by UI drag end. Converts screen -> world and deploys if valid.
        /// </summary>
        public bool TryDeployAtScreen(Vector2 screenPos)
        {
            if (_cam == null) return false;

            Vector3 w = _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
            return TryDeployAtWorld(new Vector2(w.x, w.y));
        }

        private bool TryDeployAtWorld(Vector2 worldPos)
        {
            if (SelectedSlot < 0) return false;

            // Clamp to arena
            worldPos = BattlefieldMath.ClampToArena(worldPos, _cfg);

            // Clamp into player deploy zone (so dragging slightly above boundary still drops inside)
            worldPos.y = _validator.ClampToPlayerDeployY(worldPos.y);

            // Validate deploy rule (must be in player zone)
            if (!_validator.IsPlayerDeployAllowed(worldPos))
                return false;

            // Snap to nearest lane
            var snapped = BattlefieldMath.SnapToNearestLane(worldPos, _cfg);

            // Get card
            var card = _hand.GetCardAt(SelectedSlot);
            if (card == null || card.SpawnPrefab == null) return false;

            // Energy check
            if (!_energy.TrySpend(card.EnergyCost))
                return false;

            // Spawn correct prefab for that card
            _spawner.Spawn(card.SpawnPrefab, snapped, "P");

            // Cycle the used card
            _hand.UseCardAt(SelectedSlot);

            return true;
        }
    }
}
