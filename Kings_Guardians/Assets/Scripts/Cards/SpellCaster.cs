using UnityEngine;
using KingGuardians.Cards;
using KingGuardians.Towers;
using KingGuardians.Units;

namespace KingGuardians.Combat
{
    /// <summary>
    /// Executes spells in the world.
    /// MVP: instant AoE damage.
    /// This class has no UI logic. Controller decides when/where to cast.
    /// </summary>
    public sealed class SpellCaster
    {
        /// <summary>
        /// Casts a spell at a world position.
        /// </summary>
        public void Cast(SpellDefinition spell, Vector2 worldPos)
        {
            if (spell == null) return;

            // Find all colliders in radius (2D).
            Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, spell.Radius);

            for (int i = 0; i < hits.Length; i++)
            {
                var col = hits[i];
                if (col == null) continue;

                if (spell.AffectUnits && col.TryGetComponent<UnitHealth>(out var unitHp))
                {
                    unitHp.TakeDamage(spell.Damage);
                }

                if (spell.AffectTowers && col.TryGetComponent<TowerHealth>(out var towerHp))
                {
                    towerHp.TakeDamage(spell.Damage);
                }
            }
        }
    }
}
