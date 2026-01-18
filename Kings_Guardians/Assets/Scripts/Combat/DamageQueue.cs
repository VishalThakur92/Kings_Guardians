using System.Collections.Generic;
using UnityEngine;

namespace KingGuardians.Combat
{
    /// <summary>
    /// Collects damage events during Update and applies them in LateUpdate.
    ///
    /// Why:
    /// - Ensures "simultaneous" hits resolve fairly within the same frame.
    /// - Prevents update-order bias where one unit kills the other first.
    ///
    /// MVP rule:
    /// - All damage enqueued in a frame is applied together at the end of that frame.
    /// </summary>
    public sealed class DamageQueue : MonoBehaviour
    {
        // One queued damage item.
        private struct DamageEvent
        {
            public IDamageable Target;
            public int Amount;
        }

        // List is faster than Queue for bulk processing and reuse.
        private readonly List<DamageEvent> _buffer = new List<DamageEvent>(128);

        /// <summary>
        /// Global instance (MVP).
        /// Later you can replace this with dependency injection.
        /// </summary>
        public static DamageQueue Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        /// <summary>
        /// Enqueue damage to be applied at end-of-frame.
        /// Safe to call from any attacker.
        /// </summary>
        public void Enqueue(IDamageable target, int amount)
        {
            if (target == null) return;
            if (amount <= 0) return;

            _buffer.Add(new DamageEvent { Target = target, Amount = amount });
        }

        private void LateUpdate()
        {
            // Apply all damage events collected this frame.
            // Important: we do not short-circuit after a target dies, because
            // multiple hits in the same frame should still be applied consistently.
            for (int i = 0; i < _buffer.Count; i++)
            {
                var e = _buffer[i];

                // Target may have been destroyed between enqueue and apply.
                if (e.Target == null) continue;

                e.Target.TakeDamage(e.Amount);
            }

            _buffer.Clear();
        }
    }
}
