using UnityEngine;
using KingGuardians.Units;

namespace KingGuardians.AI
{
    /// <summary>
    /// Data-driven config for the MVP enemy spawner.
    /// Keeps spawn tuning out of code and easy to iterate.
    /// </summary>
    [CreateAssetMenu(menuName = "KingGuardians/AI/Enemy Spawner Config", fileName = "EnemySpawnerConfig")]
    public sealed class EnemySpawnerConfig : ScriptableObject
    {
        [Header("Spawn Timing")]
        [Tooltip("Seconds before first enemy spawn.")]
        [Min(0f)] public float StartDelaySeconds = 1.5f;

        [Tooltip("Seconds between spawns.")]
        [Min(0.1f)] public float SpawnIntervalSeconds = 3.0f;

        [Header("Spawn Pool (MVP)")]
        [Tooltip("Enemy will spawn one of these entries each time (random).")]
        public EnemySpawnEntry[] Pool;
    }

    /// <summary>
    /// One possible enemy spawn option.
    /// </summary>
    [System.Serializable]
    public struct EnemySpawnEntry
    {
        [Tooltip("Prefab to spawn (use your Unit_Placeholder prefab for now).")]
        public GameObject Prefab;

        [Tooltip("Stats to apply after spawn (Tank/DPS/etc).")]
        public UnitStatsDefinition Stats;

        [Tooltip("Relative spawn weight for random selection.")]
        [Min(0.01f)] public float Weight;
    }
}
