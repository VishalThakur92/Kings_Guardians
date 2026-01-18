namespace KingGuardians.Units
{
    /// <summary>
    /// Whether a unit is on the ground or in the air.
    /// Used for targeting rules (e.g., some units can't hit air).
    /// </summary>
    public enum UnitDomain
    {
        Ground = 0,
        Air = 1
    }
}
