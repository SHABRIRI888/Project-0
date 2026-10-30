using System;
using System.Collections.Generic;
using UnityEngine;

public enum StatusType { Buff, Debuff }
public enum StatusEffectKind { AttackUp, DefenseUp, Shield, Cleanse, HealOverTime, Stun }

[Serializable]
public class StatusEffect
{
    public string effectName;
    public StatusType type;
    public StatusEffectKind kind;
    public float value;
    public float duration;
    public float timer;

    public StatusEffect(string name, StatusType statusType, StatusEffectKind effectKind, float val, float dur)
    {
        effectName = name;
        type = statusType;
        kind = effectKind;
        value = val;
        duration = dur;
        timer = dur;
    }
}

[Serializable]
public class CharacterStats
{
    [Header("Main Stats (Items / Base)")]
    public float maxHp = 500f;
    public float currentHp;
    public float attackDamage = 50f;
    public float defense = 20f;

    [Header("Secondary Stats (Buffs & Sets Only)")]
    public float attackSpeed = 1.0f;
    public float manaGainPerHit = 20f;
    public float currentMana = 0f;
    public const float MAX_MANA = 100f;

    [Header("Percentage Stats")]
    [Range(0f, 100f)] public float dodgeChance = 5f;
    [Range(0f, 100f)] public float critChance = 10f;
    public float critDamageBonus = 0f; // Innate +100% (200% total). Bonus adds directly.

    // Asymptotic Defense Formula: Defense / (Defense + 100)
    public float GetDamageReductionPercentage()
    {
        return (defense / (defense + 100f)) * 100f;
    }

    public float CalculateIncomingDamage(float rawDamage, out bool isDodged)
    {
        isDodged = UnityEngine.Random.Range(0f, 100f) < dodgeChance;
        if (isDodged) return 0f;

        float reductionPercent = GetDamageReductionPercentage() / 100f;
        float finalDamage = rawDamage * (1f - reductionPercent);
        return Mathf.Max(1f, finalDamage);
    }

    public float CalculateOutgoingDamage(out bool isCrit)
    {
        isCrit = UnityEngine.Random.Range(0f, 100f) < critChance;
        if (isCrit)
        {
            float totalCritMultiplier = 2.0f + (critDamageBonus / 100f);
            return attackDamage * totalCritMultiplier;
        }
        return attackDamage;
    }
}