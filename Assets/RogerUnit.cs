using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogerUnit : CharacterUnit
{
    private void Awake()
    {
        // Adjust Roger's Stats directly inside his own script
        characterName = "Roger";
        unitClass = CharacterClass.Tank;
        rarity = CharacterRarity.Uncommon;

        stats.maxHp = 600f;
        stats.currentHp = 600f;
        stats.attackDamage = 45f;
        stats.defense = 30f;
        stats.attackSpeed = 1.0f;
        stats.manaGainPerHit = 20f; // Exactly 5 hits to reach 100 Mana (20 x 5)
    }

    // Roger's Signature Ultimate: Time-Stop Triple Strike
    public override IEnumerator ExecuteUltimateRoutine(List<CharacterUnit> allies, List<CharacterUnit> enemies)
    {
        stats.currentMana = 0f; // Reset mana

        Debug.Log($"<color=yellow>==========================================</color>");
        Debug.Log($"<color=yellow>   TIME STOP! ROGER ACTIVATES ULTIMATE!  </color>");
        Debug.Log($"<color=yellow>==========================================</color>");

        // 1. Pause game time so no other actions/attacks occur
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        // Visual delay during time freeze
        yield return new WaitForSecondsRealtime(0.75f);

        // 2. Select frontmost active target
        CharacterUnit target = enemies.Find(e => e.stats.currentHp > 0 && e.gameObject.activeSelf);

        if (target != null)
        {
            // Deal 3x normal attack damage at once
            float ultimateDamage = stats.CalculateOutgoingDamage(out bool isCrit) * 3.0f;
            Debug.Log($"<color=gold>ROGER STRIKES {target.characterName} WITH TIME-STOP BLAST FOR {ultimateDamage} DAMAGE! [Crit: {isCrit}]</color>");
            target.TakeDamage(ultimateDamage);

            if (target.stats.currentHp <= 0)
            {
                Debug.Log($"<color=red>{target.characterName} was obliterated by Roger's Ultimate!</color>");
            }
        }

        yield return new WaitForSecondsRealtime(0.5f);

        // 3. Resume game time
        Time.timeScale = originalTimeScale;
        Debug.Log($"<color=yellow>--- Time resumes! ---</color>");
    }
}