using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CombatDataSO : ScriptableObject
{
    [SerializeField]
    private int health, baseHealth, orbNumber, spiritMeter, maxSpiritMeter, baseSpiritFill, hostility;
    [SerializeField]
    private float damageTakenModifier, damageGivenModifier, spiritFillModifier, baseDamageTakenMod, baseDamageGivenMod, baseSpiritFillMod,
        passiveDamageTakenModifier, vulnerableModifier;
    [SerializeField]
    private int flatDamageTakenModifier;
    [SerializeField]
    private Dictionary<string, Effect> statusEffects = new Dictionary<string, Effect>();
    [SerializeField]
    private bool isPlayer;
    [SerializeField]
    private string[] spiritAffinities;

    public void Awake()
    {
        baseHealth = 100;
        health = 100;
        spiritMeter = 0;
        maxSpiritMeter = 200;
        flatDamageTakenModifier = 0;
        passiveDamageTakenModifier = 0;
        baseDamageGivenMod = 1f;
        baseDamageTakenMod = 1f;
        baseSpiritFillMod = 1f;
        baseSpiritFill = 10;
        hostility = UnityEngine.Random.Range(40,80);
        resetModifiers();
    }

    public void resetModifiers()
    {
        damageGivenModifier = baseDamageGivenMod;
        damageTakenModifier = baseDamageTakenMod;
        spiritFillModifier = baseSpiritFillMod;
        vulnerableModifier = baseDamageTakenMod;
        flatDamageTakenModifier = 0;
    }
    /*Setter Methods*/

    public void setHealth(int value)
    {
        if (value <= 0)
        {
            Debug.Log("Unit out of HP");
        }
        health = value;
    }

    private void setSpiritMeter(int value)
    {
        if (value <= 0)
        {
            Debug.Log("Spirit Meter is Full");
        }
        spiritMeter = value;
    }

    public void setSpiritAffinities(string[] affinities)
    {
        spiritAffinities = new string[affinities.Length];
        for (int i = 0; i <  affinities.Length; i++)
        {
            spiritAffinities[i] = affinities[i];
        }
    }


    public void takeDamage(int baseDamage)
    {
        int result = (int) System.Math.Ceiling(baseDamage * ((damageTakenModifier * vulnerableModifier) + passiveDamageTakenModifier)) + flatDamageTakenModifier;
        setHealth(health - result);
    }
    public int sendDamage(int baseDamage)
    {
        return (int)System.Math.Ceiling(baseDamage * damageGivenModifier); 
    }

    public void fillSpiritMeter(int spiritMeterFillAmount)
    {
        int result = (int)System.Math.Ceiling(spiritMeterFillAmount * spiritFillModifier);
        setSpiritMeter(spiritMeter + result);

    }

    public void addStatusEffect(string effectID, Effect effect)
    {
        //sanity check, whenever a status effect is bound to be added, we must make sure that its count is full.
        effect.EffectCount = effect.EffectBaseCount;

        if (statusEffects.TryGetValue( effectID, out Effect effectValue))
        {
            if (effectValue.EffectCount < effect.EffectCount)
            {
                effectValue.EffectCount = effect.EffectCount;
            }
        }
        else
        {
            statusEffects.Add(effectID, effect);
        }
    }

    public void clearFinishedStatusEffects(List<string> effectIDs)
    {
        foreach (string effectID in effectIDs)
        {
            switch (effectID)
            {
                case "effect_defend":
                    damageTakenModifier = baseDamageTakenMod;
                    break;
                case "effect_dodge":
                    damageTakenModifier = baseDamageTakenMod;
                    break;
                case "effect_vulnerable":
                    vulnerableModifier = 1;
                    break;
                case "effect_charge":
                    damageGivenModifier = baseDamageGivenMod;
                    break;
            }
            if (statusEffects.TryGetValue(effectID, out Effect effect))
            {
                effect.EffectCount = effect.EffectBaseCount;
                statusEffects.Remove(effectID);
            }
            
        }
    }
    //Method overload for a single string
    public void clearFinishedStatusEffects(string effectID)
    {
        statusEffects[effectID].EffectCount = statusEffects[effectID].EffectBaseCount;
        statusEffects.Remove(effectID);
    }


    public bool reduceCount(string EffectID)
    {
        if (statusEffects.TryGetValue(EffectID, out Effect effect))
        {
            effect.EffectCount -= 1;
            return true;
        }
        return false;
    }

    /* Combat Stats Changing Methods */
    public void increaseMaximumSpiritBar(int value)
    {
        spiritMeter += value;
    }

    public void increaseHostility(int value)
    {
        hostility += value;
    }

    public void decreaseHostility(int value)
    {
        hostility -= value;
    }

    public void increaseHealth(int value)
    {
        health += value;
        if (health > baseHealth)
        {
            health = baseHealth;
        }
    }

    public void resetSpiritMeter()
    {
        spiritMeter = 0;
    }

    /* Modifier Changing Methods*/

    public void setDamageTakenModifier (float value)
    {
        damageTakenModifier = baseDamageTakenMod * value;
    }

    public void setVulnerableDamageTakenModifier(float value)
    {
        vulnerableModifier = baseDamageTakenMod * value;
    }

    public void changeDamageTakenModifier (float value)
    {
        damageTakenModifier += value;
    }
    public void setDamageGivenModifier(float value)
    {
        damageGivenModifier = baseDamageGivenMod * value;
    }

    public void setSpiritMeterFillModifier(float value)
    {
        spiritFillModifier = baseSpiritFillMod * value;
    }

    public void setPassiveDamageTakenModifier(float value)
    {
        passiveDamageTakenModifier += value;
    }

    public void setFlatDamageBonusModifier(int value)
    {
        flatDamageTakenModifier = value;
    }

    
    /* Getter Functions */

    public Dictionary<string, Effect> getStatusEffects()
    {
        return statusEffects;
    }

    public Effect getSpecificStatusEffect(string effectID)
    {
        return statusEffects.TryGetValue(effectID, out Effect effect) ? effect : null;
    }

    public float getHealthPercentageRemaining()
    {
        return (float)health / baseHealth;
    }

    public float getSpiritPercentageRemaining()
    {
        return (float)spiritMeter / maxSpiritMeter;
    }
    public int getHealth()
    {
        return health;
    }

    public int getSpirit()
    {
        return spiritMeter;
    }

    public int getMaxSpirit()
    {
        return maxSpiritMeter;
    }

    public string[] getSpiritAffinities()
    {
        return spiritAffinities;
    }

    public int getBaseSpiritFill()
    {
        return baseSpiritFill;
    }

    public int getHostility()
    {
        return hostility;
    }

    public bool isPlayerData()
    {
        return isPlayer;
    }

    
}
