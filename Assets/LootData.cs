using System;
using UnityEngine;

public enum Rarity { Common = 1, Uncommon = 2, Rare = 3, Epic = 4, Mythical = 5, Godly = 6 }
public enum EquipmentType { Weapon, Helmet, Chestplate, Leggings, Boots }

[Serializable]
public class Equipment
{
    public string setName;
    public EquipmentType type;
    public Rarity rarity;
    public int quality; // 1 to 100

    public float baseATK;
    public float baseHP;
    public float baseDEF;

    public Equipment(string set, EquipmentType itemType, Rarity itemRarity, int itemQuality)
    {
        setName = set;
        type = itemType;
        rarity = itemRarity;
        quality = Mathf.Clamp(itemQuality, 1, 100);

        CalculateStats();
    }

    // High Rarity beats low Rarity; Quality fine-tunes inside tier
    private void CalculateStats()
    {
        float rarityMultiplier = (int)rarity * 100f;
        float qualityMultiplier = 1.0f + ((quality - 1) / 99f) * 0.30f; // Quality adds up to 30%
        float finalStatValue = rarityMultiplier * qualityMultiplier;

        switch (type)
        {
            case EquipmentType.Weapon:
                baseATK = finalStatValue * 1.5f;
                break;
            case EquipmentType.Helmet:
                baseHP = finalStatValue * 10f;
                break;
            case EquipmentType.Chestplate:
                baseDEF = finalStatValue * 1.2f;
                baseHP = finalStatValue * 5f;
                break;
            case EquipmentType.Leggings:
                baseDEF = finalStatValue * 1.0f;
                break;
            case EquipmentType.Boots:
                baseHP = finalStatValue * 4f;
                break;
        }
    }

    public string GetFullName()
    {
        return $"{rarity} {setName}'s {type}";
    }
}