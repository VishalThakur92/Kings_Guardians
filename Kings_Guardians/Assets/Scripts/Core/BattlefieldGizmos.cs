using UnityEngine;

namespace KingGuardians.Core
{
    /// <summary>
    /// Draws battlefield debug visuals in the Scene view:
    /// - Lanes
    /// - Deploy boundaries
    /// - Arena extents
    ///
    /// This is MVP-only tooling but is extremely valuable for correctness.
    /// </summary>
    public sealed class BattlefieldGizmos : MonoBehaviour
    {
        private BattlefieldConfig _cfg;

        public void Initialize(BattlefieldConfig cfg) => _cfg = cfg;

        private void OnDrawGizmos()
        {
            if (_cfg == null) return;

            DrawArenaBounds();
            DrawLanes();
            DrawDeployBoundaries();
            DrawTowerPositions();
        }

        private void DrawArenaBounds()
        {
            Gizmos.color = Color.white;
            var center = Vector3.zero;
            var size = new Vector3(_cfg.HalfArenaLength * 2f, _cfg.LaneSpacing * Mathf.Max(1, _cfg.LaneCount), 0f);
            Gizmos.DrawWireCube(center, size);
        }

        private void DrawLanes()
        {
            Gizmos.color = Color.cyan;

            // Lanes are lines parallel to X axis at different Y offsets.
            for (int i = 0; i < _cfg.LaneCount; i++)
            {
                float y = LaneIndexToY(i);
                var a = new Vector3(-_cfg.HalfArenaLength, y, 0f);
                var b = new Vector3(_cfg.HalfArenaLength, y, 0f);
                Gizmos.DrawLine(a, b);
            }
        }

        private void DrawDeployBoundaries()
        {
            // Player A deploy boundary (left side)
            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector3(_cfg.FriendlyDeployMaxX, -50f, 0f), new Vector3(_cfg.FriendlyDeployMaxX, 50f, 0f));

            // Player B deploy boundary (right side)
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(_cfg.EnemyDeployMinX, -50f, 0f), new Vector3(_cfg.EnemyDeployMinX, 50f, 0f));
        }

        private void DrawTowerPositions()
        {
            Gizmos.color = Color.yellow;

            DrawPoint(_cfg.PlayerMainTowerPos);
            DrawPoint(_cfg.PlayerOutpostAPos);
            DrawPoint(_cfg.PlayerOutpostBPos);

            DrawPoint(_cfg.EnemyMainTowerPos);
            DrawPoint(_cfg.EnemyOutpostAPos);
            DrawPoint(_cfg.EnemyOutpostBPos);
        }

        private void DrawPoint(Vector2 p)
        {
            Gizmos.DrawSphere(new Vector3(p.x, p.y, 0f), 0.2f);
        }

        private float LaneIndexToY(int laneIndex)
        {
            // Center lanes around Y=0.
            // For 2 lanes: y = +spacing/2 and -spacing/2
            float mid = (_cfg.LaneCount - 1) * 0.5f;
            return (laneIndex - mid) * _cfg.LaneSpacing;
        }
    }
}
