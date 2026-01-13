using UnityEngine;

namespace KingGuardians.Cards
{
    /// <summary>
    /// Defines the player's deck list (MVP: just a list of CardDefinition).
    /// Hand draws/cycles from this list.
    /// </summary>
    [CreateAssetMenu(menuName = "KingGuardians/Cards/Deck Config", fileName = "DeckConfig")]
    public sealed class DeckConfig : ScriptableObject
    {
        [Min(1)] public int HandSize = 4;
        public CardDefinition[] Cards; // MVP: 8 cards recommended, but can be fewer for testing
    }
}
