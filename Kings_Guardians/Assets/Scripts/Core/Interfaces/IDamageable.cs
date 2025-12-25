namespace KingGuardians.Combat
{
    /// <summary>
    /// Something that can take damage and be destroyed.
    /// Kept minimal for MVP; later you can add armor, shields, resistances, etc.
    /// </summary>
    public interface IDamageable
    {
        bool IsAlive { get; }
        void TakeDamage(int amount);
    }
}
