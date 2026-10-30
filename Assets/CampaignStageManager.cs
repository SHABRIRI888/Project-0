using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignStageManager : MonoBehaviour
{
    public CharacterUnit playerHero;
    public LootGenerator lootGenerator;

    public int currentStage = 1;

    public void StartCampaign()
    {
        StartCoroutine(RunCampaignLoop());
    }

    IEnumerator RunCampaignLoop()
    {
        while (currentStage <= 20)
        {
            // Reset Hero Health before starting any new stage
            playerHero.gameObject.SetActive(true);
            playerHero.stats.currentHp = playerHero.stats.maxHp;
            playerHero.stats.currentMana = 0f;

            Debug.Log($"<color=yellow>==========================================</color>");
            Debug.Log($"<color=yellow>   STARTING STAGE 1.{currentStage} (Roger HP Reset: {playerHero.stats.maxHp})  </color>");
            Debug.Log($"<color=yellow>==========================================</color>");

            bool stageSuccess = true;

            for (int wave = 1; wave <= 4; wave++)
            {
                List<CharacterUnit> waveEnemies = GetFixedWaveEnemies(currentStage, wave);
                yield return StartCoroutine(ExecuteWave(wave, waveEnemies));

                if (playerHero.stats.currentHp <= 0)
                {
                    stageSuccess = false;
                    break;
                }
            }

            if (stageSuccess)
            {
                Debug.Log($"<color=green>STAGE 1.{currentStage} CLEARED!</color>");

                bool isMajorBoss = (currentStage % 5 == 0);
                bool isMiniBoss = (!isMajorBoss);
                List<Equipment> drops = lootGenerator.GenerateStageLoot(isMajorBoss, isMiniBoss);

                Debug.Log("<color=cyan>--- STAGE LOOT DROPS ---</color>");
                foreach (var item in drops)
                {
                    Debug.Log($"Dropped: {item.GetFullName()} | Quality: {item.quality}/100");
                }

                currentStage++;
                yield return new WaitForSeconds(2.0f);
            }
            else
            {
                Debug.Log($"<color=red>==========================================</color>");
                Debug.Log($"<color=red>DEFEAT AT STAGE 1.{currentStage}! Roger was defeated.</color>");
                Debug.Log($"<color=red>Tip: Recruit more team members and equip dropped gear!</color>");
                Debug.Log($"<color=red>==========================================</color>");
                yield break;
            }
        }
    }

    // Hardcoded, static stage designs with progressive difficulty
    private List<CharacterUnit> GetFixedWaveEnemies(int stage, int wave)
    {
        List<CharacterUnit> enemies = new List<CharacterUnit>();

        int fighters = 0;
        int archers = 0;
        int knights = 0;

        if (stage == 1) // Stage 1.1: Easy Intro
        {
            if (wave == 1) fighters = 1;
            else if (wave == 2) { fighters = 1; archers = 1; }
            else if (wave == 3) fighters = 2;
            else if (wave == 4) knights = 1;
        }
        else if (stage == 2) // Stage 1.2: Ranged Pressure
        {
            if (wave == 1) { fighters = 1; archers = 1; }
            else if (wave == 2) { fighters = 1; archers = 2; }
            else if (wave == 3) { fighters = 2; archers = 1; }
            else if (wave == 4) { knights = 1; fighters = 1; }
        }
        else if (stage == 3) // Stage 1.3: Heavy Frontline
        {
            if (wave == 1) fighters = 2;
            else if (wave == 2) { fighters = 2; archers = 1; }
            else if (wave == 3) fighters = 3;
            else if (wave == 4) { knights = 1; archers = 1; }
        }
        else if (stage == 4) // Stage 1.4: Full Squad Assault (Roger solo limit)
        {
            if (wave == 1) { fighters = 2; archers = 1; }
            else if (wave == 2) { fighters = 2; archers = 2; }
            else if (wave == 3) { fighters = 3; archers = 1; }
            else if (wave == 4) { knights = 1; fighters = 1; archers = 1; }
        }
        else if (stage == 5) // Stage 1.5: Major Boss Stage
        {
            if (wave == 1) { fighters = 2; archers = 2; }
            else if (wave == 2) { fighters = 3; archers = 1; }
            else if (wave == 3) knights = 2;
            else if (wave == 4)
            {
                // Standalone Boss
                GameObject bossObj = new GameObject("GoblinBerserker_Boss");
                enemies.Add(Chapter1EnemyData.CreateEnemy(bossObj, Chapter1EnemyType.GoblinBerserker, stage));
                return enemies;
            }
        }

        // Spawn Mobs for non-boss waves
        for (int i = 0; i < fighters; i++)
        {
            GameObject obj = new GameObject($"GoblinFighter_{i}");
            enemies.Add(Chapter1EnemyData.CreateEnemy(obj, Chapter1EnemyType.GoblinFighter, stage));
        }
        for (int i = 0; i < archers; i++)
        {
            GameObject obj = new GameObject($"GoblinArcher_{i}");
            enemies.Add(Chapter1EnemyData.CreateEnemy(obj, Chapter1EnemyType.GoblinArcher, stage));
        }
        for (int i = 0; i < knights; i++)
        {
            GameObject obj = new GameObject($"GoblinKnight_{i}");
            enemies.Add(Chapter1EnemyData.CreateEnemy(obj, Chapter1EnemyType.GoblinKnight, stage));
        }

        return enemies;
    }

    IEnumerator ExecuteWave(int waveNum, List<CharacterUnit> enemies)
    {
        Debug.Log($"<color=orange>--- WAVE {waveNum}/4 START ---</color>");

        while (playerHero.stats.currentHp > 0 && HasAliveEnemies(enemies))
        {
            List<CharacterUnit> heroTeam = new List<CharacterUnit> { playerHero };

            if (playerHero.isReadyForUltimate)
            {
                yield return StartCoroutine(playerHero.ExecuteUltimateRoutine(heroTeam, enemies));
            }

            foreach (var enemy in enemies)
            {
                if (enemy.gameObject.activeSelf && enemy.stats.currentHp > 0)
                {
                    enemy.PerformAutoAttack();
                    playerHero.TakeDamage(enemy.stats.attackDamage);

                    if (playerHero.stats.currentHp <= 0) yield break;
                }
            }

            CharacterUnit target = GetFirstAliveEnemy(enemies);
            if (target != null)
            {
                playerHero.PerformAutoAttack();
                target.TakeDamage(playerHero.stats.attackDamage);
            }

            yield return new WaitForSeconds(1.0f);
        }

        foreach (var enemy in enemies) Destroy(enemy.gameObject);
        yield return new WaitForSeconds(1.0f);
    }

    private bool HasAliveEnemies(List<CharacterUnit> list) => list.Exists(e => e.stats.currentHp > 0 && e.gameObject.activeSelf);
    private CharacterUnit GetFirstAliveEnemy(List<CharacterUnit> list) => list.Find(e => e.stats.currentHp > 0 && e.gameObject.activeSelf);
}