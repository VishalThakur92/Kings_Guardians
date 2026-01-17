using UnityEngine;

namespace KingGuardians.Units
{
    /// <summary>
    /// Data-only definition of unit stats for MVP.
    /// Keeps tuning out of code and avoids prefab duplication.
    /// </summary>
    [CreateAssetMenu(menuName = "KingGuardians/Units/Unit Stats", fileName = "UnitStats_")]
    public sealed class UnitStatsDefinition : ScriptableObject
    {
        [Header("Core")]
        [Min(1)] public int MaxHp = 120;

        [Header("Movement")]
        [Min(0f)] public float MoveSpeed = 2.0f;

        [Header("Attack")]
        [Min(0)] public int DamagePerHit = 25;
        [Min(0.05f)] public float AttackInterval = 0.75f;
    }
}
