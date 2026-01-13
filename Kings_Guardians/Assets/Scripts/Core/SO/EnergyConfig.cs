using UnityEngine;

namespace KingGuardians.Core
{
    /// <summary>
    /// Tunable energy settings for MVP.
    /// Use ScriptableObject so designers can adjust without code changes.
    /// </summary>
    [CreateAssetMenu(menuName = "KingGuardians/Configs/Energy Config", fileName = "EnergyConfig")]
    public sealed class EnergyConfig : ScriptableObject
    {
        [Min(0)] public int StartEnergy = 10;
        [Min(1)] public int MaxEnergy = 50;

        [Tooltip("Energy gained per second (e.g., 0.5 => +1 energy every 2 seconds).")]
        [Min(0.01f)] public float RegenPerSecond = 0.5f;
    }
}
