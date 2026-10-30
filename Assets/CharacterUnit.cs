using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterClass { Tank, Warrior, Ranger, Mage, Buffer }
public enum CharacterRarity { Uncommon, Rare, Epic, Mythical, Godly }

public class CharacterUnit : MonoBehaviour
{
    public string characterName;
    public CharacterClass unitClass;
    public CharacterRarity rarity;

    // Explicit initialization prevents null reference errors
    public CharacterStats stats = new CharacterStats();

    public List<StatusEffect> activeStatuses = new List<StatusEffect>();
    public float activeShield = 0f;

    private float attackTimer = 0f;
    public bool isReadyForUltimate => stats.currentMana >= CharacterStats.MAX_MANA;

    protected virtual void Start()
    {
        if (stats.currentHp <= 0) stats.currentHp = stats.maxHp;
    }

    protected virtual void Update()
    {
        if (stats.currentHp <= 0) return;

        attackTimer += Time.deltaTime;
        float attackInterval = 1.0f / Mathf.Max(0.1f, stats.attackSpeed);

        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;
            PerformAutoAttack();
        }

        UpdateStatuses(Time.deltaTime);
    }

    public virtual void PerformAutoAttack()
    {
        float damage = stats.CalculateOutgoingDamage(out bool isCrit);
        stats.currentMana = Mathf.Min(CharacterStats.MAX_MANA, stats.currentMana + stats.manaGainPerHit);

        Debug.Log($"{characterName} ({unitClass}) dealt {damage} DMG! [Crit: {isCrit}] | Mana: {stats.currentMana}/100");
    }

    // Virtual Ultimate routine designed to be overridden by individual character scripts
    public virtual IEnumerator ExecuteUltimateRoutine(List<CharacterUnit> allies, List<CharacterUnit> enemies)
    {
        stats.currentMana = 0f;
        yield return null;
    }

    public virtual void TakeDamage(float rawDamage)
    {
        float finalDamage = stats.CalculateIncomingDamage(rawDamage, out bool isDodged);

        if (isDodged)
        {
            Debug.Log($"<color=green>{characterName} DODGED!</color>");
            return;
        }

        if (activeShield > 0)
        {
            if (activeShield >= finalDamage)
            {
                activeShield -= finalDamage;
                return;
            }
            else
            {
                finalDamage -= activeShield;
                activeShield = 0;
            }
        }

        stats.currentHp -= finalDamage;
        if (stats.currentHp <= 0) gameObject.SetActive(false);
    }

    private void UpdateStatuses(float deltaTime)
    {
        for (int i = activeStatuses.Count - 1; i >= 0; i--)
        {
            activeStatuses[i].timer -= deltaTime;
            if (activeStatuses[i].timer <= 0) activeStatuses.RemoveAt(i);
        }
    }
}