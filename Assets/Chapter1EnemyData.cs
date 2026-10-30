using UnityEngine;

public enum Chapter1EnemyType
{
    GoblinFighter,   // Balanced melee mob
    GoblinArcher,    // Low HP, High DMG ranged mob
    GoblinKnight,    // Mini-boss (Tanky + Heavy damage)
    GoblinBerserker, // Stage 1.5 Boss
    GoblinExecutioner, // Stage 1.10 Boss
    GoblinBeast,     // Stage 1.15 Boss
    GoblinKing       // Stage 1.20 Final Chapter Boss
}

public static class Chapter1EnemyData
{
    public static CharacterUnit CreateEnemy(GameObject parent, Chapter1EnemyType type, int stageIndex)
    {
        CharacterUnit unit = parent.AddComponent<CharacterUnit>();

        // Multiplier based on stage index (Stage 1 = 1.0x, Stage 20 = 3.5x)
        float stageScale = 1.0f + ((stageIndex - 1) * 0.13f);

        switch (type)
        {
            case Chapter1EnemyType.GoblinFighter:
                unit.characterName = "Goblin Fighter";
                unit.unitClass = CharacterClass.Warrior;
                unit.rarity = CharacterRarity.Uncommon;
                unit.stats.maxHp = 140f * stageScale;
                unit.stats.attackDamage = 22f * stageScale;
                unit.stats.defense = 6f * stageScale;
                unit.stats.attackSpeed = 0.85f;
                unit.stats.manaGainPerHit = 15f;
                break;

            case Chapter1EnemyType.GoblinArcher:
                unit.characterName = "Goblin Archer";
                unit.unitClass = CharacterClass.Ranger;
                unit.rarity = CharacterRarity.Uncommon;
                unit.stats.maxHp = 90f * stageScale;  // Squishy
                unit.stats.attackDamage = 35f * stageScale; // High ATK
                unit.stats.defense = 2f * stageScale;
                unit.stats.attackSpeed = 1.1f;
                unit.stats.manaGainPerHit = 15f;
                break;

            case Chapter1EnemyType.GoblinKnight:
                unit.characterName = "Goblin Knight";
                unit.unitClass = CharacterClass.Tank;
                unit.rarity = CharacterRarity.Rare;
                unit.stats.maxHp = 420f * stageScale;
                unit.stats.attackDamage = 38f * stageScale;
                unit.stats.defense = 18f * stageScale;
                unit.stats.attackSpeed = 1.0f;
                unit.stats.manaGainPerHit = 20f;
                break;

            case Chapter1EnemyType.GoblinBerserker: // Stage 1.5 Boss
                unit.characterName = "Goblin Berserker";
                unit.unitClass = CharacterClass.Warrior;
                unit.rarity = CharacterRarity.Epic;
                unit.stats.maxHp = 1400f;
                unit.stats.attackDamage = 110f;
                unit.stats.defense = 25f;
                unit.stats.attackSpeed = 1.25f;
                unit.stats.manaGainPerHit = 25f;
                break;

            case Chapter1EnemyType.GoblinExecutioner: // Stage 1.10 Boss
                unit.characterName = "Goblin Executioner";
                unit.unitClass = CharacterClass.Warrior;
                unit.rarity = CharacterRarity.Epic;
                unit.stats.maxHp = 2600f;
                unit.stats.attackDamage = 175f;
                unit.stats.defense = 40f;
                unit.stats.attackSpeed = 1.0f;
                unit.stats.manaGainPerHit = 25f;
                break;

            case Chapter1EnemyType.GoblinBeast: // Stage 1.15 Boss
                unit.characterName = "Goblin Beast";
                unit.unitClass = CharacterClass.Tank;
                unit.rarity = CharacterRarity.Mythical;
                unit.stats.maxHp = 4200f;
                unit.stats.attackDamage = 230f;
                unit.stats.defense = 65f;
                unit.stats.attackSpeed = 0.9f;
                unit.stats.manaGainPerHit = 25f;
                break;

            case Chapter1EnemyType.GoblinKing: // Stage 1.20 Boss
                unit.characterName = "Goblin King";
                unit.unitClass = CharacterClass.Warrior;
                unit.rarity = CharacterRarity.Godly;
                unit.stats.maxHp = 6500f;
                unit.stats.attackDamage = 310f;
                unit.stats.defense = 85f;
                unit.stats.attackSpeed = 1.15f;
                unit.stats.manaGainPerHit = 30f;
                break;
        }

        unit.stats.currentHp = unit.stats.maxHp;
        return unit;
    }
}