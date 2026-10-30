using System;
using System.Collections.Generic;
using UnityEngine;

public class LootGenerator : MonoBehaviour
{
    public List<string> availableSets = new List<string> { "Adventurer", "Knight", "Dragon", "Shadow", "Celestial" };

    public Rarity RollRarity(bool isMajorBoss, bool isMiniBoss)
    {
        int roll = UnityEngine.Random.Range(1, 101);

        if (isMajorBoss)
        {
            if (roll <= 5) return Rarity.Godly;     // 5%
            if (roll <= 20) return Rarity.Mythical;  // 15%
            if (roll <= 50) return Rarity.Epic;      // 30%
            if (roll <= 80) return Rarity.Rare;      // 30%
            return Rarity.Uncommon;                   // 20%
        }
        else if (isMiniBoss)
        {
            if (roll <= 1) return Rarity.Godly;     // 1%
            if (roll <= 6) return Rarity.Mythical;  // 5%
            if (roll <= 20) return Rarity.Epic;      // 14%
            if (roll <= 50) return Rarity.Rare;      // 30%
            if (roll <= 85) return Rarity.Uncommon;  // 35%
            return Rarity.Common;                     // 15%
        }
        else
        {
            if (roll <= 1) return Rarity.Epic;      // 1%
            if (roll <= 10) return Rarity.Rare;      // 9%
            if (roll <= 40) return Rarity.Uncommon;  // 30%
            return Rarity.Common;                     // 60%
        }
    }

    public int RollQuality()
    {
        float randomVal = UnityEngine.Random.value;
        return Mathf.FloorToInt(Mathf.Pow(randomVal, 2.5f) * 99f) + 1;
    }

    public List<Equipment> GenerateStageLoot(bool isMajorBoss, bool isMiniBoss)
    {
        List<Equipment> lootList = new List<Equipment>();
        Dictionary<EquipmentType, int> typeCounts = new Dictionary<EquipmentType, int>();

        foreach (EquipmentType type in Enum.GetValues(typeof(EquipmentType)))
        {
            typeCounts[type] = 0;
        }

        while (lootList.Count < 5)
        {
            EquipmentType randomType = (EquipmentType)UnityEngine.Random.Range(0, 5);

            // Constraint Rule: Max 3 of the same type
            if (typeCounts[randomType] >= 3) continue;

            Rarity rarity = RollRarity(isMajorBoss, isMiniBoss);
            int quality = RollQuality();
            string set = availableSets[UnityEngine.Random.Range(0, availableSets.Count)];

            Equipment newItem = new Equipment(set, randomType, rarity, quality);
            typeCounts[randomType]++;
            lootList.Add(newItem);
        }

        return lootList;
    }
}