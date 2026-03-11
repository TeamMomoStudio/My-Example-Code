using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GearRoller
{
    public static GearInstance Create(ItemDataSO baseItem, ItemRarity rarity, List<AffixSO> pool)
    {
        var gear = new GearInstance { baseItemId = baseItem.id, rarity = rarity };

        // กรอง Affix ที่ตรงสายและมี Weight > 0
        var available = pool.Where(a => a != null && a.allowedType == baseItem.itemType && a.weight > 0).ToList();
        
        //  กำหนดจำนวน Affix ตาม Rarity 
        int want = rarity switch { ItemRarity.Normal => 2, ItemRarity.Magic => 3, ItemRarity.Rare => 4, _ => 0 };
        
        //  สุ่ม Affix
        for (int i = 0; i < Mathf.Min(want, available.Count); i++)
        {
            var picked = PickWeighted(available);
            available.Remove(picked); 

            gear.affixIds.Add(picked.affixId);
            foreach (var r in picked.rolls)
            {
                gear.rolledStats.Add(new RolledStat { stat = r.stat, value = Random.Range(r.min, r.max) });
            }
        }
        return gear;
    }

    // อัลกอริทึมสุ่มน้ำหนัก (Weighted Random) เขียนแบบย่อด้วย LINQ
    static AffixSO PickWeighted(List<AffixSO> list)
    {
        int roll = Random.Range(0, list.Sum(x => x.weight)), acc = 0;
        return list.FirstOrDefault(a => (acc += a.weight) > roll) ?? list[0];
    }
}