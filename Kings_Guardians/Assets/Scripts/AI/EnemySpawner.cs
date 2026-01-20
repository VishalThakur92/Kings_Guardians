using KingGuardians.Core;
using KingGuardians.Units;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace KingGuardians.AI
{
    /// <summary>
    /// MVP enemy spawner:
    /// - Spawns enemy units at a fixed interval
    /// - Chooses lane (alternate/random)
    /// - Applies stats to the spawned unit
    /// - Sets unit identity to Enemy
    /// - Sets motor direction downward
    /// </summary>
    public sealed class EnemySpawner : MonoBehaviour
    {
        [Header("Configs")]
        [SerializeField] private BattlefieldConfig battlefieldConfig;
        [SerializeField] private EnemySpawnerConfig spawnerConfig;
        [SerializeField] private EnemySpawnBehaviorConfig behaviorConfig;

        [Header("Hierarchy")]
        [SerializeField] private Transform unitsRoot;

        private float _nextSpawnTime;
        private int _laneCursor;

        private void Awake()
        {
            // Validate required references early.
            if (battlefieldConfig == null || spawnerConfig == null || behaviorConfig == null)
            {
                Debug.LogError("[EnemySpawner] Missing config references.", this);
                enabled = false;
                return;
            }

            if (unitsRoot == null)
                unitsRoot = this.transform;

            // Schedule the first spawn.
            _nextSpawnTime = Time.time + spawnerConfig.StartDelaySeconds;
        }

        private void Update()
        {
            if (Time.time < _nextSpawnTime) return;

            SpawnOne();
            _nextSpawnTime = Time.time + spawnerConfig.SpawnIntervalSeconds;
        }

        private void SpawnOne()
        {
            var entry = PickWeightedEntry(spawnerConfig.Pool);
            if (entry.Prefab == null)
            {
                Debug.LogWarning("[EnemySpawner] Pool entry prefab is null.");
                return;
            }

            // Choose lane index
            int laneIndex = ChooseLaneIndex();

            // Compute spawn position in enemy deploy zone:
            // Enemy side is TOP. So spawn near +HalfArenaHeight, inset downward slightly.
            float laneX = BattlefieldMath.LaneIndexToX(laneIndex, battlefieldConfig.LaneCount, battlefieldConfig.LaneSpacing);
            float spawnY = battlefieldConfig.HalfArenaHeight - behaviorConfig.SpawnInsetY;

            Vector2 spawnPos = new Vector2(laneX, spawnY);

            // Spawn
            var go = Instantiate(entry.Prefab, new Vector3(spawnPos.x, spawnPos.y, 0f), Quaternion.identity, unitsRoot);
            go.name = $"E_{go.GetInstanceID()}";

            // Ensure identity is Enemy
            var identity = go.GetComponent<UnitIdentity>();
            if (identity == null) identity = go.AddComponent<UnitIdentity>();
            identity.Init(TeamId.Enemy);

            // Ensure health exists and apply stats
            var health = go.GetComponent<UnitHealth>();
            if (health == null) health = go.AddComponent<UnitHealth>();
            if (entry.Stats != null) health.ApplyMaxHp(entry.Stats.MaxHp);

            // Ensure attack exists and apply stats
            var attack = go.GetComponent<UnitAttack>();
            if (attack == null) attack = go.AddComponent<UnitAttack>();
            if (entry.Stats != null) { 
                attack.ApplyAttackStats(entry.Stats.DamagePerHit, entry.Stats.AttackInterval);
                attack.ApplyAttackRange(entry.Stats.AttackRange); 
            }

            // Ensure motor exists, apply speed and move DOWN
            var motor = go.GetComponent<UnitMotor>();
            if (motor == null) motor = go.AddComponent<UnitMotor>();
            if (entry.Stats != null) motor.ApplyMoveSpeed(entry.Stats.MoveSpeed);
            motor.SetDirection(Vector2.down);

            // IMPORTANT:
            // Your UnitTargeter currently has Team default = Player.
            // Ensure it is set correctly for enemies.
            var targeter = go.GetComponent<UnitTargeter>();
            if (targeter == null)
            {
                // If your prefab already has UnitTargeter, it will exist.
                // If not, add it so enemy units can engage towers too.
                targeter = go.AddComponent<UnitTargeter>();
            }
            targeter.SetTeam(TeamId.Enemy);
        }

        private int ChooseLaneIndex()
        {
            int laneCount = Mathf.Max(1, battlefieldConfig.LaneCount);

            if (behaviorConfig.Mode == EnemySpawnBehaviorConfig.LaneMode.Random)
            {
                return Random.Range(0, laneCount);
            }

            // Alternate
            int idx = _laneCursor % laneCount;
            _laneCursor++;
            return idx;
        }

        private EnemySpawnEntry PickWeightedEntry(EnemySpawnEntry[] pool)
        {
            // Fallback if pool not configured
            if (pool == null || pool.Length == 0)
                return default;

            float total = 0f;
            for (int i = 0; i < pool.Length; i++)
                total += Mathf.Max(0f, pool[i].Weight);

            if (total <= 0f)
                return pool[Random.Range(0, pool.Length)];

            float roll = Random.value * total;
            float accum = 0f;

            for (int i = 0; i < pool.Length; i++)
            {
                accum += Mathf.Max(0f, pool[i].Weight);
                if (roll <= accum)
                    return pool[i];
            }

            return pool[pool.Length - 1];
        }
    }
}
