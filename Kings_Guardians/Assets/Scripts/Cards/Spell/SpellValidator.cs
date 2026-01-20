using UnityEngine;
using KingGuardians.Cards;
using KingGuardians.Core;

namespace KingGuardians.Combat
{
    /// <summary>
    /// Validates whether a spell can be cast at a world position.
    /// MVP rules are simple and deterministic.
    /// </summary>
    public sealed class SpellValidator
    {
        private readonly BattlefieldConfig _cfg;

        public SpellValidator(BattlefieldConfig cfg)
        {
            _cfg = cfg;
        }

        public bool CanCastAt(SpellDefinition spell, Vector2 worldPos)
        {
            if (spell == null) return false;

            // Must be inside arena bounds
            if (!IsInsideArena(worldPos)) return false;

            switch (spell.CastRule)
            {
                case SpellCastRule.AnywhereInsideArena:
                    return true;

                case SpellCastRule.PlayerSideOnly:
                    // Player side is bottom half (Y <= PlayerDeployMaxY is deploy zone,
                    // but spell "player side" we consider the whole bottom half of arena)
                    return worldPos.y <= 0f;

                case SpellCastRule.EnemySideOnly:
                    return worldPos.y >= 0f;

                default:
                    return false;
            }
        }

        private bool IsInsideArena(Vector2 p)
        {
            return p.x >= -_cfg.HalfArenaWidth && p.x <= _cfg.HalfArenaWidth &&
                   p.y >= -_cfg.HalfArenaHeight && p.y <= _cfg.HalfArenaHeight;
        }
    }
}
