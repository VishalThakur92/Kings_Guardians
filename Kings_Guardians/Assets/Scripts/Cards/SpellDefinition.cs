using UnityEngine;

namespace KingGuardians.Cards
{
    /// <summary>
    /// Data-only spell definition.
    /// MVP: Fireball-style instant AoE damage.
    /// </summary>
    [CreateAssetMenu(menuName = "KingGuardians/Cards/Spell Definition", fileName = "Spell_")]
    public sealed class SpellDefinition : ScriptableObject
    {
        [Header("Cast Rules")]
        [Tooltip("Can this spell be cast anywhere, or only on enemy side, etc.")]
        public SpellCastRule CastRule = SpellCastRule.AnywhereInsideArena;

        [Header("AoE (MVP)")]
        [Min(0.1f)] public float Radius = 1.5f;
        [Min(0)] public int Damage = 40;

        [Header("Targets")]
        public bool AffectUnits = true;
        public bool AffectTowers = false; // keep false for MVP unless you want it
    }

    public enum SpellCastRule
    {
        AnywhereInsideArena = 0,
        PlayerSideOnly = 1,
        EnemySideOnly = 2
    }
}
