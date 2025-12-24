using UnityEngine;

namespace KingGuardians.Core
{
    /// <summary>
    /// Pure data describing the battlefield layout.
    /// Keep this free of scene references so it can be reused in tests and future server simulation.
    /// </summary>
    [CreateAssetMenu(menuName = "KingGuardians/Configs/Battlefield Config", fileName = "BattlefieldConfig")]
    public sealed class BattlefieldConfig : ScriptableObject
    {
        [Header("Lane Layout")]
        [Tooltip("Number of lanes (MVP: 2).")]
        [Min(1)] public int LaneCount = 2;

        [Tooltip("Distance between lane center lines (world units).")]
        [Min(0.1f)] public float LaneSpacing = 3.5f;

        [Tooltip("Half-length of the arena along the X axis. Units typically move +X or -X.")]
        [Min(1f)] public float HalfArenaLength = 10f;

        [Header("Deploy Zones")]
        [Tooltip("Players can only deploy on their side. This is the X boundary for the friendly deploy zone.")]
        public float FriendlyDeployMaxX = -1f; // For Player A (left side) in local mode

        [Tooltip("For Player B (right side) in local mode.")]
        public float EnemyDeployMinX = 1f;

        [Header("Towers")]
        [Tooltip("Local-side main tower position (Player A).")]
        public Vector2 PlayerMainTowerPos = new Vector2(-9f, 0f);

        [Tooltip("Local-side outpost A position (Player A).")]
        public Vector2 PlayerOutpostAPos = new Vector2(-6.5f, 2f);

        [Tooltip("Local-side outpost B position (Player A).")]
        public Vector2 PlayerOutpostBPos = new Vector2(-6.5f, -2f);

        [Tooltip("Enemy main tower position (Player B).")]
        public Vector2 EnemyMainTowerPos = new Vector2(9f, 0f);

        [Tooltip("Enemy outpost A position (Player B).")]
        public Vector2 EnemyOutpostAPos = new Vector2(6.5f, 2f);

        [Tooltip("Enemy outpost B position (Player B).")]
        public Vector2 EnemyOutpostBPos = new Vector2(6.5f, -2f);
    }
}
