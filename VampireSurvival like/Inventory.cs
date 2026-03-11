using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
public event Action OnInventoryChanged;

    [Header("Config")]
    [SerializeField] private int capacity = 20;

    [Header("Runtime Data")]
    [SerializeField] private List<InventorySlot> slots = new();

    private PlayerData player;

    private void Awake()
    {
        player = GetComponent<PlayerData>();

        slots.Clear();
        for (int i = 0; i < capacity; i++)
            slots.Add(new InventorySlot());
    }

    public bool AddItem(ItemDataSO item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        // 1) เติมของลงใน Stack เดิมที่ยังไม่เต็มก่อน
        foreach (var slot in slots)
        {
            if (slot.IsEmpty || slot.item != item) continue;

            int spaceLeft = item.MaxStack - slot.amount;
            if (spaceLeft > 0)
            {
                int addAmount = Mathf.Min(spaceLeft, amount);
                slot.amount += addAmount;
                amount -= addAmount; // หักลบจำนวนที่เพิ่งใส่ไป

                // ถ้าของหมดมือแล้ว แปลว่าเพิ่มสำเร็จ
                if (amount <= 0)
                {
                    SortInventory();
                    return true;
                }
            }
        }

        // 2) ถ้ายังมีของเหลือ ให้หาช่องว่างช่องใหม่
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty) continue;

            slot.item = item;
            int addAmount = Mathf.Min(item.MaxStack, amount);
            slot.amount = addAmount;
            amount -= addAmount;

            if (amount <= 0)
            {
                SortInventory();
                return true;
            }
        }

        // กรณีช่องเต็มหมดแล้ว แต่ยังมีของเหลือค้างอยู่
        SortInventory(); 
        return amount == 0;
    }

    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return;

        var slot = slots[slotIndex];
        if (slot.IsEmpty) return;

        var item = slot.item;

        if (item is not IUsableItem usable) return;

        usable.Use(player);

        if (item.itemType == ItemType.Consumable)
        {
            slot.amount--;
            if (slot.amount <= 0)
                slot.Clear();

            SortInventory();
        }

        Debug.Log($"Using item: {item.name}, type: {item.itemType}");
    }

    public bool RemoveItem(ItemDataSO item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        // เช็คก่อนว่ามีไอเทมรวมทั้งหมดพอให้ลบหรือไม่
        int totalOwned = slots.Where(s => !s.IsEmpty && s.item == item).Sum(s => s.amount);
        if (totalOwned < amount) return false; // ของไม่พอให้ลบ

        foreach (var slot in slots)
        {
            if (slot.IsEmpty || slot.item != item) continue;

            if (slot.amount >= amount)
            {
                slot.amount -= amount;
                amount = 0; // ลบครบแล้ว
            }
            else
            {
                amount -= slot.amount; // หักยอดที่ต้องการลบออกตามของที่มีในช่องนี้
                slot.amount = 0; 
            }

            if (slot.amount <= 0) slot.Clear();

            if (amount <= 0) break; // ถ้าครบแล้วให้ออกจากลูป
        }

        NotifyChange();
        return true;
    }
    private void NotifyChange() => OnInventoryChanged?.Invoke();
}