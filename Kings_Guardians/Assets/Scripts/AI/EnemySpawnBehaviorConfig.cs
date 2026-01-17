using UnityEngine;

namespace KingGuardians.AI
{
    /// <summary>
    /// Controls how enemy chooses lanes and exact spawn position.
    /// </summary>
    [CreateAssetMenu(menuName = "KingGuardians/AI/Enemy Spawn Behavior", fileName = "EnemySpawnBehaviorConfig")]
    public sealed class EnemySpawnBehaviorConfig : ScriptableObject
    {
        public enum LaneMode
        {
            Alternate,
            Random
        }

        [Header("Lane Selection")]
        public LaneMode Mode = LaneMode.Alternate;

        [Header("Spawn Position")]
        [Tooltip("How far inside the enemy deploy zone to spawn (prevents spawning exactly on boundary).")]
        [Min(0f)] public float SpawnInsetY = 0.75f;
    }
}
