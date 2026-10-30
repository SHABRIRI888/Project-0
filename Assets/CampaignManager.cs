using System.Collections.Generic;
using UnityEngine;

public class CampaignManager : MonoBehaviour
{
    public int chapter = 1;
    public int stage = 1;
    public int currentWave = 1;

    public LootGenerator lootGenerator;

    public string GetStageName() => $"{chapter}.{stage}";
    public bool IsBossStage() => (stage % 5 == 0); // 1.5, 1.10, 1.15, 1.20

    public void ClearWave()
    {
        if (currentWave < 4)
        {
            currentWave++;
            Debug.Log($"Advancing to Wave {currentWave}/4 in Stage {GetStageName()}");
        }
        else
        {
            Debug.Log($"<color=green>STAGE {GetStageName()} CLEARED!</color>");

            // Generate stage drops
            bool boss = IsBossStage();
            bool miniBoss = (!boss && currentWave == 4);
            List<Equipment> drops = lootGenerator.GenerateStageLoot(boss, miniBoss);

            foreach (var item in drops)
            {
                Debug.Log($"Loot Drop: {item.GetFullName()} | Quality: {item.quality}/100");
            }

            AdvanceStage();
        }
    }

    private void AdvanceStage()
    {
        currentWave = 1;
        if (stage >= 20)
        {
            chapter++;
            stage = 1;
        }
        else
        {
            stage++;
        }
        Debug.Log($"Next Stage: {GetStageName()}");
    }

    // Evaluates 2-piece/3-piece stats and 5-piece unique passive effect
    public void ApplySetBonuses(CharacterUnit unit, List<Equipment> equippedGear)
    {
        Dictionary<string, int> setCounts = new Dictionary<string, int>();

        foreach (var gear in equippedGear)
        {
            if (gear == null) continue;
            if (setCounts.ContainsKey(gear.setName)) setCounts[gear.setName]++;
            else setCounts[gear.setName] = 1;
        }

        foreach (var pair in setCounts)
        {
            if (pair.Value >= 2) unit.stats.maxHp += 100f; // 2-Piece Stats
            if (pair.Value >= 3) unit.stats.attackDamage += 25f; // 3-Piece Stats
            if (pair.Value >= 5)
            {
                Debug.Log($"<color=magenta>5-PIECE UNIQUE SET PASSIVE ACTIVATED on {unit.characterName}!</color>");
            }
        }
    }
}