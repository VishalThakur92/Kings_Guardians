using UnityEngine;

namespace KingGuardians.Core
{
    /// <summary>
    /// Validates whether a deployment position is allowed.
    /// For MVP: player can deploy ONLY below PlayerDeployMaxY (green line).
    ///
    /// Later, this becomes server-authoritative validation.
    /// </summary>
    public sealed class DeploymentValidator
    {
        private readonly BattlefieldConfig _cfg;

        public DeploymentValidator(BattlefieldConfig cfg)
        {
            _cfg = cfg;
        }

        /// <summary>
        /// Returns true if this world position is inside the player's deploy zone.
        /// </summary>
        public bool IsPlayerDeployAllowed(Vector2 worldPos)
        {
            // Must be inside arena bounds (basic safety).
            bool insideArena =
                worldPos.x >= -_cfg.HalfArenaWidth && worldPos.x <= _cfg.HalfArenaWidth &&
                worldPos.y >= -_cfg.HalfArenaHeight && worldPos.y <= _cfg.HalfArenaHeight;

            if (!insideArena) return false;

            // Player deploy zone in portrait: bottom side ends at PlayerDeployMaxY
            return worldPos.y <= _cfg.PlayerDeployMaxY;
        }

        /// <summary>
        /// Clamps a deployment Y into the player deploy region (optional convenience).
        /// Useful when user taps slightly above boundary.
        /// </summary>
        public float ClampToPlayerDeployY(float y)
        {
            // Player zone: from -HalfArenaHeight up to PlayerDeployMaxY
            return Mathf.Clamp(y, -_cfg.HalfArenaHeight, _cfg.PlayerDeployMaxY);
        }
    }
}
