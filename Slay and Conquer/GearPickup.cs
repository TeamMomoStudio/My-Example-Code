using UnityEngine;

public static class GearValue
{
    // ใช้คำนวณตัวคูณทีเดียว
    static int Mult(ItemRarity r) => r switch { ItemRarity.Magic => 2, ItemRarity.Rare => 4, _ => 1 };

    // คืนค่าออกมาเป็น Tuple ทั้ง Gold และ Astra  
    public static (int sellGold, int salvageAstra) GetValues(ItemDataSO baseItem, ItemRarity rarity)
    {
        int m = Mult(rarity);
        return baseItem switch
        {
            WeaponItemSO w => (w.sellGold * m, w.salvageAstra * m),
            ArmorItemSO a => (a.sellGold * m, a.salvageAstra * m),
            _ => (0, 0)
        };
    }
}

public class GearPickup : MonoBehaviour
{
    private GearInstance gear;

    public void Init(GearInstance gear, ItemDatabaseSO db)
    {
        this.gear = gear;
        var baseItem = db.GetItem(gear.baseItemId);
        
      
        if (baseItem && TryGetComponent(out SpriteRenderer sr)) 
            sr.sprite = baseItem.icon;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // เช็ค Inventory และแอดของ ถ้าสำเร็จค่อยทำลายทิ้ง
        if (other.TryGetComponent(out Inventory inventory))
        {
            inventory.AddGear(gear); 
            Destroy(gameObject);
        }
    }
}