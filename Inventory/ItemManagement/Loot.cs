using UnityEngine;
using System.Collections;

/// <summary>
/// This class handles Lootable items + their random generation
/// </summary>
public class Loot {

    public Item lootItem;

    public void GenerateLootItem()
    {
        lootItem = ItemDatabaseManager.Instance.GetItem((ItemType)Random.Range(0, ItemType.GetNames(typeof(ItemType)).Length), 0);
        lootItem.quality = (ItemQualityType)Random.Range(0, ItemQualityType.GetNames(typeof(ItemQualityType)).Length);
        lootItem.uniqueID = ItemDatabaseManager.Instance.GenerateUniqueID();
        int statCount = Random.Range(2, 7);

        for (int i = 0; i < statCount; i++)
        {
            lootItem.statModifiers.Add(CreateRandomStatModifier());
        }
    }

    private StatModifier CreateRandomStatModifier()
    {
        return new StatModifier((StatType)Random.Range(0, StatType.GetNames(typeof(StatType)).Length), Random.Range(1, 10));
    }
}
