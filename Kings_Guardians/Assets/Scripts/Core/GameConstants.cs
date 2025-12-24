namespace KingGuardians.Core
{
    /// <summary>
    /// Global constants for the MVP.
    /// Keep constants here only. Use ScriptableObjects for tunable values.
    /// </summary>
    public static class GameConstants
    {
        public const int LaneCount = 2;

        // MVP placeholder values — tune later via configs.
        public const int MaxEnergy = 50;
        public const float EnergyRegenPerSecond = 0.5f; // example: 1 energy per 2 seconds

        public const float MatchDurationSeconds = 180f;
        public const float OvertimeSeconds = 60f;

        // If you later use a grid for placement, define tile size here.
        public const float TileSize = 1f;
    }
}
