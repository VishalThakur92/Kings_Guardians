using UnityEngine;
using KingGuardians.Cards;
using KingGuardians.UI;

namespace KingGuardians.Core
{
    /// <summary>
    /// Composition root for Phase 2 systems (energy + hand + card deploy).
    /// </summary>
    public sealed class Phase2Installer : MonoBehaviour
    {
        [Header("Configs")]
        [SerializeField] private BattlefieldConfig battlefieldConfig;
        [SerializeField] private EnergyConfig energyConfig;
        [SerializeField] private DeckConfig deckConfig;

        [Header("Prefabs/Roots")]
        [SerializeField] private GameObject unitSpawnPrefab; // optional: can be taken from card
        [SerializeField] private Transform unitsRoot;

        [Header("Scene Refs")]
        [SerializeField] private BattlefieldTapInput battlefieldTapInput;
        [SerializeField] private EnergyBarView energyBarView;
        [SerializeField] private CardSlotView[] cardSlots; // size 4

        private EnergySystem _energy;
        private HandModel _hand;
        private CardDeploymentController _deployController;
        private DeploymentValidator _deployValidator;
        private UnitSpawner _spawner;

        private void Awake()
        {
            // Validate
            if (battlefieldConfig == null || energyConfig == null || deckConfig == null)
            {
                Debug.LogError("[Phase2Installer] Missing configs.", this);
                enabled = false;
                return;
            }

            if (battlefieldTapInput == null || energyBarView == null || cardSlots == null || cardSlots.Length == 0)
            {
                Debug.LogError("[Phase2Installer] Missing scene references (tap input / energy bar / card slots).", this);
                enabled = false;
                return;
            }

            if (unitsRoot == null) unitsRoot = transform;

            // Build core runtime objects
            _energy = new EnergySystem(energyConfig);
            _hand = new HandModel(deckConfig);

            _deployValidator = new DeploymentValidator(battlefieldConfig);

            // Spawner uses prefab from card; UnitSpawner only instantiates.
            // We'll pass a dummy prefab here and override per card in future if needed.
            // For now UnitSpawner will instantiate whatever prefab is in the card via CardDeploymentController,
            // but our UnitSpawner currently instantiates a single prefab. To keep it minimal:
            // Use UnitSpawner only as a generic instantiator; we’ll update it in the next step if needed.
            _spawner = new UnitSpawner(unitsRoot);

            // Build deploy controller
            _deployController = new CardDeploymentController(
                battlefieldConfig,
                _deployValidator,
                _spawner,
                _energy,
                _hand
            );

            // Hook input
            battlefieldTapInput.Initialize(_deployController);

            // Hook energy UI
            _energy.OnChanged += energyBarView.Set;

            // Hook hand UI
            _hand.OnHandChanged += RefreshHandUI;
            RefreshHandUI();

            // Wire card clicks (do NOT assume array order == slot index)
            for (int i = 0; i < cardSlots.Length; i++)
            {
                var slotView = cardSlots[i];
                int handIndex = slotView.SlotIndex;

                slotView.Button.onClick.AddListener(() =>
                {
                    _deployController.SelectSlot(handIndex);
                    RefreshSelectionUI();
                    RefreshHandUI(); // refresh interactable based on energy
                });
            }

            // Default select slot 0
            _deployController.SelectSlot(0);
            RefreshSelectionUI();
        }

        private void Update()
        {
            // Tick energy regen
            _energy.Tick(Time.deltaTime);

            // Update hand interactability based on current energy (lightweight)
            RefreshHandInteractableOnly();
        }

        private void RefreshHandUI()
        {
            for (int i = 0; i < cardSlots.Length; i++)
            {
                var slotView = cardSlots[i];
                int handIndex = slotView.SlotIndex;

                var card = _hand.GetCardAt(handIndex);

                slotView.SetIcon(card != null ? card.Icon : null);
                slotView.SetCost(card != null ? card.EnergyCost : 0);
            }

            RefreshHandInteractableOnly();
        }

        private void RefreshHandInteractableOnly()
        {
            for (int i = 0; i < cardSlots.Length; i++)
            {
                var slotView = cardSlots[i];
                int handIndex = slotView.SlotIndex;

                var card = _hand.GetCardAt(handIndex);
                bool can = (card != null) && _energy.CanSpend(card.EnergyCost);
                slotView.SetInteractable(can);
            }
        }

        private void RefreshSelectionUI()
        {
            for (int i = 0; i < cardSlots.Length; i++)
            {
                var slotView = cardSlots[i];
                slotView.SetSelected(slotView.SlotIndex == _deployController.SelectedSlot);
            }
        }
    }
}
