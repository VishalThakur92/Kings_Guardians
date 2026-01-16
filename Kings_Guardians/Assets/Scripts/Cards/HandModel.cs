using System;
using System.Collections.Generic;

namespace KingGuardians.Cards
{
    /// <summary>
    /// Maintains the active hand and cycling order.
    /// Pure logic, no Unity objects.
    /// </summary>
    public sealed class HandModel : ICardProvider
    {
        public event Action OnHandChanged;

        private readonly List<CardDefinition> _deckQueue;
        private readonly CardDefinition[] _hand;

        public int HandSize => _hand.Length;

        public HandModel(DeckConfig deck)
        {
            // Build a queue we cycle through.
            _deckQueue = new List<CardDefinition>(deck.Cards);
            _hand = new CardDefinition[deck.HandSize];

            // Initial draw
            for (int i = 0; i < _hand.Length; i++)
                _hand[i] = DrawNext();

            OnHandChanged?.Invoke();
        }

        public CardDefinition GetCardAt(int index) => _hand[index];

        public CardDefinition UseCardAt(int index)
        {
            var used = _hand[index];
            _hand[index] = DrawNext();
            OnHandChanged?.Invoke();
            return used;
        }

        private CardDefinition DrawNext()
        {
            // MVP: simple cycle: take first, move to back.
            var next = _deckQueue[0];
            _deckQueue.RemoveAt(0);
            _deckQueue.Add(next);
            return next;
        }
    }
}
