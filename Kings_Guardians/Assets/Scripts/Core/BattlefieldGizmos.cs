using UnityEngine;

namespace KingGuardians.Core
{
    /// <summary>
    /// Portrait battlefield debug visuals:
    /// - Arena bounds (width x height)
    /// - Lanes as vertical columns (lines parallel to Y axis)
    /// - Deploy boundaries as horizontal lines (Y thresholds)
    /// - Tower points
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

            // Portrait arena: width along X, height along Y.
            var center = Vector3.zero;
            var size = new Vector3(_cfg.HalfArenaWidth * 2f, _cfg.HalfArenaHeight * 2f, 0f);
            Gizmos.DrawWireCube(center, size);
        }

        private void DrawLanes()
        {
            Gizmos.color = Color.cyan;

            // Portrait lanes: vertical columns (lines parallel to Y axis at different X offsets).
            for (int i = 0; i < _cfg.LaneCount; i++)
            {
                float x = LaneIndexToX(i);
                var a = new Vector3(x, -_cfg.HalfArenaHeight, 0f);
                var b = new Vector3(x, _cfg.HalfArenaHeight, 0f);
                Gizmos.DrawLine(a, b);
            }
        }

        private void DrawDeployBoundaries()
        {
            // Player deploy boundary (bottom side ends at PlayerDeployMaxY)
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                new Vector3(-50f, _cfg.PlayerDeployMaxY, 0f),
                new Vector3(50f, _cfg.PlayerDeployMaxY, 0f)
            );

            // Enemy deploy boundary (top side starts at EnemyDeployMinY)
            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                new Vector3(-50f, _cfg.EnemyDeployMinY, 0f),
                new Vector3(50f, _cfg.EnemyDeployMinY, 0f)
            );
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

        private float LaneIndexToX(int laneIndex)
        {
            // Center lanes around X=0.
            // For 2 lanes: x = -spacing/2 and +spacing/2
            float mid = (_cfg.LaneCount - 1) * 0.5f;
            return (laneIndex - mid) * _cfg.LaneSpacing;
        }
    }
}
