using KingGuardians.Units;
using UnityEngine;

namespace KingGuardians.Cards
{

    /// <summary>
    /// Defines a playable card in the deck.
    /// MVP: only unit spawn + cost + icon.
    /// Later: spells/buildings, rarity, upgrades, etc.
    /// </summary>
    [CreateAssetMenu(menuName = "KingGuardians/Cards/Card Definition", fileName = "Card_")]
    public sealed class CardDefinition : ScriptableObject
    {

        [Header("Unit Stats (MVP)")]
        public UnitStatsDefinition UnitStats;
    
        [Header("UI")]
        public string DisplayName;
        public Sprite Icon;

        [Header("Gameplay")]
        [Min(0)] public int EnergyCost = 3;

        [Tooltip("Prefab spawned when this card is deployed (MVP: unit).")]
        public GameObject SpawnPrefab;
    }
}
