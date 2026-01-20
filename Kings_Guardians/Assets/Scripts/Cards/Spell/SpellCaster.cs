using KingGuardians.Cards;
using KingGuardians.Towers;
using KingGuardians.Units;
using KingGuardians.VFX;
using UnityEngine;

namespace KingGuardians.Combat
{
    /// <summary>
    /// Executes spells in the world.
    /// MVP: instant AoE damage + optional one-shot VFX.
    /// </summary>
    public sealed class SpellCaster
    {
        /// <summary>
        /// Casts a spell at a world position.
        /// </summary>
        public void Cast(SpellDefinition spell, Vector2 worldPos)
        {
            if (spell == null) return;

            // --- VFX (optional) ---
            // Spawn a basic visual indicator so players can see the spell impact.
            if (spell.CastVfxPrefab != null)
            {
                var vfxGo = Object.Instantiate(spell.CastVfxPrefab, new Vector3(worldPos.x, worldPos.y, 0f), Quaternion.identity);

                // If it has our helper component, initialize radius scaling.
                var oneShot = vfxGo.GetComponent<OneShotSpellVfx>();
                if (oneShot != null)
                    oneShot.InitRadius(spell.Radius);
            }

            // --- Damage (instant AoE) ---
            Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, spell.Radius);

            for (int i = 0; i < hits.Length; i++)
            {
                var col = hits[i];
                if (col == null) continue;

                if (spell.AffectUnits && col.TryGetComponent<UnitHealth>(out var unitHp))
                    unitHp.TakeDamage(spell.Damage);

                if (spell.AffectTowers && col.TryGetComponent<TowerHealth>(out var towerHp))
                    towerHp.TakeDamage(spell.Damage);
            }
        }
    }
}
