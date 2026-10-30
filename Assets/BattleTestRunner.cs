using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTestRunner : MonoBehaviour
{
    [Header("Hero Reference")]
    public CharacterUnit heroRoger;

    private List<CharacterUnit> activeEnemies = new List<CharacterUnit>();

    void Start()
    {
        StartCoroutine(RunStage1_1());
    }

    IEnumerator RunStage1_1()
    {
        Debug.Log("<color=yellow>========== STARTING STAGE 1.1 ==========</color>");

        // Wave 1: 1 Goblin
        yield return StartCoroutine(ExecuteWave(1, SpawnGoblins(1, "Goblin", hp: 120, atk: 20, def: 5, spd: 0.8f)));

        // Wave 2: 1 Goblin
        yield return StartCoroutine(ExecuteWave(2, SpawnGoblins(1, "Goblin", hp: 140, atk: 22, def: 5, spd: 0.8f)));

        // Wave 3: 2 Goblins
        yield return StartCoroutine(ExecuteWave(3, SpawnGoblins(2, "Goblin", hp: 150, atk: 25, def: 8, spd: 0.9f)));

        // Wave 4: Goblin Knight (Mini-Boss)
        yield return StartCoroutine(ExecuteWave(4, SpawnGoblins(1, "Goblin Knight (Mini-Boss)", hp: 450, atk: 40, def: 20, spd: 1.1f, isBoss: true)));

        if (heroRoger != null && heroRoger.stats.currentHp > 0)
        {
            Debug.Log("<color=green>==========================================</color>");
            Debug.Log("<color=green>STAGE 1.1 CLEAR! Roger defeated the Goblin Knight!</color>");
            Debug.Log("<color=green>==========================================</color>");
        }
    }

    IEnumerator ExecuteWave(int waveNum, List<CharacterUnit> enemies)
    {
        activeEnemies = enemies;
        Debug.Log($"<color=orange>--- WAVE {waveNum}/4 START ---</color>");

        while (heroRoger.stats.currentHp > 0 && HasAliveEnemies(activeEnemies))
        {
            List<CharacterUnit> heroTeam = new List<CharacterUnit> { heroRoger };

            // Trigger Roger's Ultimate if ready
            if (heroRoger.isReadyForUltimate)
            {
                yield return StartCoroutine(heroRoger.ExecuteUltimateRoutine(heroTeam, activeEnemies));
            }

            // Enemies Attack
            foreach (var enemy in activeEnemies)
            {
                if (enemy.gameObject.activeSelf && enemy.stats.currentHp > 0)
                {
                    enemy.PerformAutoAttack();
                    heroRoger.TakeDamage(enemy.stats.attackDamage);

                    if (heroRoger.stats.currentHp <= 0)
                    {
                        Debug.Log("<color=red>ROGER HAS BEEN DEFEATED IN STAGE 1.1!</color>");
                        yield break;
                    }
                }
            }

            // Roger Auto Attacks
            CharacterUnit target = GetFirstAliveEnemy(activeEnemies);
            if (target != null)
            {
                heroRoger.PerformAutoAttack();
                target.TakeDamage(heroRoger.stats.attackDamage);

                if (target.stats.currentHp <= 0)
                {
                    Debug.Log($"<color=red>{target.characterName} has been slain!</color>");
                }
            }

            yield return new WaitForSeconds(1.0f);
        }

        foreach (var enemy in activeEnemies) Destroy(enemy.gameObject);
        activeEnemies.Clear();

        yield return new WaitForSeconds(1.5f);
    }

    private List<CharacterUnit> SpawnGoblins(int count, string name, float hp, float atk, float def, float spd, bool isBoss = false)
    {
        List<CharacterUnit> goblinList = new List<CharacterUnit>();

        for (int i = 0; i < count; i++)
        {
            GameObject obj = new GameObject($"{name}_{i + 1}");
            CharacterUnit unit = obj.AddComponent<CharacterUnit>();

            unit.characterName = count > 1 ? $"{name} {i + 1}" : name;
            unit.unitClass = isBoss ? CharacterClass.Warrior : CharacterClass.Ranger;
            unit.rarity = isBoss ? CharacterRarity.Rare : CharacterRarity.Uncommon;

            unit.stats.maxHp = hp;
            unit.stats.currentHp = hp;
            unit.stats.attackDamage = atk;
            unit.stats.defense = def;
            unit.stats.attackSpeed = spd;
            unit.stats.manaGainPerHit = 15f;

            goblinList.Add(unit);
        }

        return goblinList;
    }

    private bool HasAliveEnemies(List<CharacterUnit> enemies)
    {
        return enemies.Exists(e => e.stats.currentHp > 0 && e.gameObject.activeSelf);
    }

    private CharacterUnit GetFirstAliveEnemy(List<CharacterUnit> enemies)
    {
        return enemies.Find(e => e.stats.currentHp > 0 && e.gameObject.activeSelf);
    }
}