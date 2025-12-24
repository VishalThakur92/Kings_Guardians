using UnityEngine;

namespace KingGuardians.Core
{
    /// <summary>
    /// Pure math helpers for the battlefield.
    /// Kept static and side-effect free for testability and future server use.
    /// </summary>
    public static class BattlefieldMath
    {
        /// <summary>
        /// Returns the lane center X position for a given lane index (portrait lanes = vertical columns).
        /// </summary>
        public static float LaneIndexToX(int laneIndex, int laneCount, float laneSpacing)
        {
            float mid = (laneCount - 1) * 0.5f;
            return (laneIndex - mid) * laneSpacing;
        }

        /// <summary>
        /// Finds the nearest lane index for a given world X position.
        /// </summary>
        public static int NearestLaneIndex(float worldX, int laneCount, float laneSpacing)
        {
            // Convert X to an approximate lane index around the centered lane grid.
            // For 2 lanes (spacing 3): lane X = -1.5 and +1.5
            // This formula works for any laneCount >= 1.
            float mid = (laneCount - 1) * 0.5f;
            float approx = (worldX / laneSpacing) + mid;

            int idx = Mathf.RoundToInt(approx);
            return Mathf.Clamp(idx, 0, laneCount - 1);
        }

        /// <summary>
        /// Snaps a world position onto the nearest lane center X, keeping Y as-is.
        /// </summary>
        public static Vector2 SnapToNearestLane(Vector2 worldPos, BattlefieldConfig cfg)
        {
            int lane = NearestLaneIndex(worldPos.x, cfg.LaneCount, cfg.LaneSpacing);
            float snappedX = LaneIndexToX(lane, cfg.LaneCount, cfg.LaneSpacing);
            return new Vector2(snappedX, worldPos.y);
        }

        /// <summary>
        /// Clamps a position into arena bounds (useful safety for MVP).
        /// </summary>
        public static Vector2 ClampToArena(Vector2 worldPos, BattlefieldConfig cfg)
        {
            float x = Mathf.Clamp(worldPos.x, -cfg.HalfArenaWidth, cfg.HalfArenaWidth);
            float y = Mathf.Clamp(worldPos.y, -cfg.HalfArenaHeight, cfg.HalfArenaHeight);
            return new Vector2(x, y);
        }
    }
}
