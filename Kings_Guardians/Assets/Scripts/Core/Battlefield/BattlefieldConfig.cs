using UnityEngine;

namespace KingGuardians.Core
{
    /// <summary>
    /// Battlefield layout data for a PORTRAIT game:
    /// - Combat flows along the Y axis (bottom -> top).
    /// - Lanes are vertical columns separated along the X axis.
    /// - Deployment zones are separated by horizontal boundary lines (Y).
    /// </summary>
    [CreateAssetMenu(menuName = "KingGuardians/Configs/Battlefield Config", fileName = "BattlefieldConfig")]
    public sealed class BattlefieldConfig : ScriptableObject
    {
        [Header("Lane Layout (Portrait)")]
        [Tooltip("Number of lanes (MVP: 2).")]
        [Min(1)] public int LaneCount = 2;

        [Tooltip("Distance between lane center columns along X (world units).")]
        [Min(0.1f)] public float LaneSpacing = 3.0f;

        [Tooltip("Half-width of the arena along the X axis.")]
        [Min(1f)] public float HalfArenaWidth = 5f;

        [Tooltip("Half-height of the arena along the Y axis. Units move bottom->top.")]
        [Min(1f)] public float HalfArenaHeight = 10f;

        [Header("Deploy Zones (Portrait)")]
        [Tooltip("Player deploy zone ends at this Y (boundary line). Player side is below this line.")]
        public float PlayerDeployMaxY = -1f;

        [Tooltip("Enemy deploy zone starts at this Y (boundary line). Enemy side is above this line.")]
        public float EnemyDeployMinY = 1f;

        [Header("Towers (Portrait)")]
        [Tooltip("Player (bottom) main tower position.")]
        public Vector2 PlayerMainTowerPos = new Vector2(0f, -9f);

        [Tooltip("Player (bottom) outpost A position.")]
        public Vector2 PlayerOutpostAPos = new Vector2(-2f, -6.5f);

        [Tooltip("Player (bottom) outpost B position.")]
        public Vector2 PlayerOutpostBPos = new Vector2(2f, -6.5f);

        [Tooltip("Enemy (top) main tower position.")]
        public Vector2 EnemyMainTowerPos = new Vector2(0f, 9f);

        [Tooltip("Enemy (top) outpost A position.")]
        public Vector2 EnemyOutpostAPos = new Vector2(-2f, 6.5f);

        [Tooltip("Enemy (top) outpost B position.")]
        public Vector2 EnemyOutpostBPos = new Vector2(2f, 6.5f);
    }
}
