using System;
using System.Collections.Generic;
using UnityEngine;
public enum StatType
{
    DamageFlat,
    DamagePercent,
    AttackSpeedPercent, // เอาไปลด interval ภายหลัง
    RangeFlat,
    CritRateFlat,       // +% crit
    HpFlat,
    AOE
}
public enum ItemRarity { Normal, Magic, Rare }

[Serializable] public struct StatRoll { public StatType stat; public float min, max; }
[Serializable] public struct RolledStat { public StatType stat; public float value; }

[CreateAssetMenu(menuName = "Game/Affix")]
public class AffixSO : ScriptableObject
{
    public int affixId;
    public string affixName;
    public ItemType allowedType;
    [Min(0)] public int weight = 100;
    public StatRoll[] rolls;
}

[CreateAssetMenu(menuName = "Game/Affix Database")]
public class AffixDatabaseSO : ScriptableObject
{
    public List<AffixSO> affixes = new();
}

[Serializable]
public class GearInstance
{
    public string uid = Guid.NewGuid().ToString("N"); // สร้าง UID ครั้งเดียวตั้งแต่เกิด
    public int baseItemId;
    public ItemRarity rarity;
    public List<int> affixIds = new();
    public List<RolledStat> rolledStats = new();
}