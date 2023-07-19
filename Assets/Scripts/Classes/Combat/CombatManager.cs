using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private RitualSetSO[] ritualSets = new RitualSetSO[5];
    [SerializeField] private ItemsSO[] items = new ItemsSO[3];
    [SerializeField] private RitualManager ritualManager;
    private bool isPlayerTurn = true;

    private int currentRitualSetIndex = 0;
    private RitualSO chosenRitual;

    private bool actionMatchesAffinity = false;
    private int spiritFillBuffer = 0;

    [SerializeField] private CombatUIManager combatUIManager;
    [SerializeField] private EnemyAISO enemyAI;
    [SerializeField] private CombatDataSO playerCombatData;
    [SerializeField] private CombatDataSO enemyCombatData;
    [SerializeField] private Dictionary<string, bool> passiveFlags = new Dictionary<string, bool>()
    {
        {"passiveOriginOne", false },
        {"passiveCallingOne", false},
        {"passiveCallingTwo", false},
        {"passiveLinkDamage", false },
        {"passiveJourneyOne", false },
        {"passiveJourneyThree", false },
        {"passiveOrdealTwo", false }
    };
    private Dictionary<string, Effect[]> passiveEffectLookupTable = new Dictionary<string, Effect[]>();

    private void Awake()
    {
        playerCombatData.Awake();
        enemyCombatData.Awake();
        enemyCombatData.setSpiritAffinities(new string[] {"affinity_green", "affinity_red", "affinity_green", "affinity_white", "affinity_black"});

        setRitualChoices();
        setItemChoices();
        combatUIManager.updateSpiritOrbsUI(enemyCombatData.getSpiritAffinities());
    }

    private void setItemChoices()
    {
        string[] itemNames = new string[items.Length];
        string[] itemAffinities = new string[items.Length];

        for (int i = 0; i < items.Length; i++)
        {
            itemNames[i] = items[i].getItemName();
            itemAffinities[i] = items[i].getAffinity();
            //remove this in-game, should be loaded from memory/save
            items[i].resetCount();
        }
        combatUIManager.updateItemButtons(itemNames, itemAffinities);
    }

    private void setRitualChoices()
    {
        RitualSO[] currentRitualSet = ritualSets[currentRitualSetIndex].getRitualSet();
        string[] ritualNames = new string[3];
        string[] ritualPassivesNames = new string[3];
        for (int i = 0; i < 3; i++)
        {
            ritualNames[i] = currentRitualSet[i].getRitualName();
            ritualPassivesNames[i] = currentRitualSet[i].getPassiveDescription();
        }
        combatUIManager.updateRitualsUI(ritualNames, ritualPassivesNames);
    }
    public void makeAction(int actionIndex)
    {
        ActionSO chosenAction = chosenRitual.getActionSet()[actionIndex];
        if (!chosenAction.getCanBeReused())
        {
            if (chosenAction.getIsUsedUp())
            {
                return;
            } else
            {
                combatUIManager.disableActionButton(actionIndex);
                chosenAction.useUp();
            }
        }
        //pushing operation
        foreach (Effect effect in chosenAction.getEffects())
        {
            playerCombatData.addStatusEffect(effect.EffectID, effect);
        }
        
        spiritFillBuffer += processAffinities(chosenAction.getAffinity(), true); //spiritFillBuffer keeps track of all pre-modified spirit meter fill

        executeStatusEffects(playerCombatData.getStatusEffects(), true);

        if (passiveFlags["passiveOriginOne"])
        {
            if (playerCombatData.getHealthPercentageRemaining() >= .5f && enemyCombatData.getHealthPercentageRemaining() >= .5f)
            {
                Debug.Log("Passive Effect: Consecration - Ceremonial Variant Activated! 20 more Spirit Meter filled.");
                spiritFillBuffer += 20;
            }
        }

        if (passiveFlags["passiveJourneyOne"])
        {
            if (playerCombatData.getHealthPercentageRemaining() >= .5f)
            {
                Debug.Log("Passive Effect: Methodical Sigil Branding Activated! 10 more Spirit Meter filled.");
                spiritFillBuffer += 10;
            }
        }

        fillSpiritMeter(spiritFillBuffer); //placed here so that any spirit meter doubling/modifying effects are applied first.
        nextTurn(); //this next turn always goes to enemy turn.
    }
    public void useItem(int itemIndex)
    {
        ItemsSO chosenItem = items[itemIndex];
        
        if (!playerCombatData.reduceCount("effect_item_free"))
        {
            if (chosenItem.getCurrentCount() > 0)
            {
                chosenItem.reduceItemCount();
                if (chosenItem.getCurrentCount() == 0)
                {
                    combatUIManager.disableActionButton(itemIndex);
                }
            }
        }
        foreach (Effect effect in chosenItem.getEffects())
        {
            if (effect.EffectTargetsPlayer)
            {
                playerCombatData.addStatusEffect(effect.EffectID, effect);
            }
            if (effect.EffectTargetsEnemy)
            {
                enemyCombatData.addStatusEffect(effect.EffectID, effect);
            }
            
        }
        executeStatusEffects(playerCombatData.getStatusEffects(), true);
        executeStatusEffects(enemyCombatData.getStatusEffects(), false);

        spiritFillBuffer += processAffinities(chosenItem.getAffinity(), false);
        spiritFillBuffer += processTags(chosenItem.getItemTags());

        fillSpiritMeter(spiritFillBuffer);

        nextTurn();
    }

    public void doDamage(int damageAmt, CombatDataSO target)
    {
        if (passiveFlags["passiveLinkDamage"] && target == enemyCombatData)
        {
            Debug.Log("Passive Effect: Puppet Piercing: Indiscriminate Jabs Activated! Spirit Meter filled by raw pre-status effect damage.");
            spiritFillBuffer += damageAmt;
        }

        target.takeDamage(damageAmt);

        //charge always procs
        target.reduceCount("effect_charge");
        
        if (!target.reduceCount("effect_dodge")) //dodge does not proc vulnerable
        {
            target.reduceCount("effect_defense");
            target.reduceCount("effect_vulnerable");
        }

        if (target.isPlayerData())
        {
            combatUIManager.updatePlayerHP(target.getHealth());
            combatUIManager.updatePlayerHPBar(target.getHealthPercentageRemaining());
        } else
        {
            combatUIManager.updateEnemyHP(target.getHealth());
            combatUIManager.updateEnemyHPBar(target.getHealthPercentageRemaining());
        }
        
    }

    private void fillSpiritMeter(int amount)
    {
        enemyCombatData.fillSpiritMeter(amount);
        combatUIManager.updateEnemySpirit(enemyCombatData.getSpirit());
        combatUIManager.updateSpiritBar(enemyCombatData.getSpiritPercentageRemaining());
        spiritFillBuffer = 0;
    }

    private void moveToNextSpiritOrb()
    {
        currentRitualSetIndex += 1;
        enemyCombatData.resetSpiritMeter();
        fillSpiritMeter(0); //shorthand for resetting the spirit meter
        setRitualChoices();
        combatUIManager.showRitualScreen();
    }
    private void runEffect(Effect effect)
    {
        CombatDataSO target = playerCombatData;
        CombatDataSO caster = enemyCombatData;
        if (effect.EffectTargetsEnemy)
        {
            caster = playerCombatData;
            target = enemyCombatData;
        }
        switch(effect.EffectID) {
            case "effect_charge":
                target.setDamageGivenModifier((float)2);
                break;
            case "effect_damage":
                Debug.Log("damage is being cast: " + caster.sendDamage(effect.EffectValue_Int).ToString() + " by " + caster.ToString()) ;
                doDamage(caster.sendDamage(effect.EffectValue_Int), target);
                break;
            case "effect_defend":
                target.setDamageTakenModifier((float) .5);
                break;
            case "effect_defense_reduce":
                target.setPassiveDamageTakenModifier(effect.EffectValue_Float);
                break;
            case "effect_dodge":
                target.setDamageTakenModifier((float)0);
                break;
            case "effect_flat_damage_bonus":
                target.setFlatDamageBonusModifier(effect.EffectValue_Int);
                break;
            case "effect_heal":
                target.increaseHealth(effect.EffectValue_Int);
                break;
            case "effect_hostility_add":
                enemyCombatData.increaseHostility(effect.EffectValue_Int);
                break;
            case "effect_hostility_reduce":
                enemyCombatData.decreaseHostility(effect.EffectValue_Int);
                break;
            case "effect_item_free":
                Debug.Log("Item System not yet implemented");
                break;
            case "effect_increase_spirit_meter":
                enemyCombatData.increaseMaximumSpiritBar(effect.EffectValue_Int);
                break;
            case "effect_spirit_fill":
                spiritFillBuffer += effect.EffectValue_Int;
                break;
            case "effect_spirit_fill_mult":
                if (actionMatchesAffinity)
                {
                    enemyCombatData.setSpiritMeterFillModifier(effect.EffectValue_Int);
                }
                break;
            case "effect_vulnerable":
                target.setVulnerableDamageTakenModifier((float)2);
                break;
        }
    }

    private void executeStatusEffects(Dictionary<string, Effect> effects, bool isPlayerData)
    {
        string[] effectIDs = new string[] {
            "effect_item_free",
            "effect_charge",
            "effect_defend",
            "effect_vulnerable",
            "effect_defense_reduce",
            "effect_flat_damage_bonus",
            "effect_dodge",
            "effect_heal",
            "effect_spirit_fill_mult",
            "effect_increase_spirit_meter",
            "effect_damage",
            "effect_spirit_fill",
            "effect_hostility_reduce",
            "effect_hostility_add",
        };

        List<string> finishedEffects = new List<string>();
        foreach (string effectID in effectIDs)
        {
            if (effects.TryGetValue(effectID, out Effect effect))
            {
                // effects that aren't per instance of damage get procced. otherwise, they are procced via doDamage().
                if (effect.EffectCount < 0)
                {
                    finishedEffects.Add(effectID);
                    continue;
                }
                
                if (effectID != "effect_defend" || effectID != "effect_dodge" || effectID != "effect_charge" || effectID != "effect_vulnerable")
                {
                    effect.EffectCount -= 1;
                }
                

                runEffect(effect);

                if (effect.EffectCount < 0)
                {
                    //push effect into effect list of things to remove
                    finishedEffects.Add(effectID);
                }
            }
        }

        if (isPlayerData)
        {
            clearFinishedStatusEffects(playerCombatData, finishedEffects);
        } else
        {
            clearFinishedStatusEffects(enemyCombatData, finishedEffects);
        }
        

    }

    private void clearFinishedStatusEffects(CombatDataSO target, List<string> effectIDs)
    {
        target.clearFinishedStatusEffects(effectIDs);
    }
    // method overload to accept only a single string.
    private void clearFinishedStatusEffects(CombatDataSO target, string effectID)
    {
        target.clearFinishedStatusEffects(effectID);
    }

    private int processAffinities(string affinity, bool isAction)
    {
        actionMatchesAffinity = false;
        int spiritFill = playerCombatData.getBaseSpiritFill();
        if (isAction)
        {
            spiritFill += 10;
        }
        if (affinity == enemyCombatData.getSpiritAffinities()[currentRitualSetIndex])
        {
            Debug.Log("Action has the same affinity! Spirit Meter fill doubled.");
            spiritFill *= 2;
            actionMatchesAffinity = true;
        }

        if (passiveFlags["passiveCallingOne"])
        {
            if (actionMatchesAffinity)
            {
                Debug.Log("Passive Effect: Puppet Piercing: Heartseeking Acupuncture Activated! 10 more Spirit Meter filled.");
                spiritFill += 10;
            }
        }

        if (passiveFlags["passiveOrdealTwo"])
        {
            if (actionMatchesAffinity)
            {
                Debug.Log("Passive Effect: Standard Protocol Binding Spell Activated! All Spirit Meter fill negated.");
                spiritFill = 0;
            }
        }


        return spiritFill;
    }

    private int processTags(string[] tags)
    {
        int spiritFill = 0;
        foreach(string tag in tags)
        {
            foreach(string theme in chosenRitual.getRitualTags())
            {
                if (tag == theme)
                {
                    spiritFill += 20;
                    break;
                }
            }
        }
        return spiritFill;
    }
    private void nextTurn ()
    {
        foreach (KeyValuePair<string,Effect> effect in playerCombatData.getStatusEffects())
        {
            Debug.Log("Player Effect: " + effect.Key + " with count: " + effect.Value.EffectCount + " and value:" + effect.Value.EffectValue_Int);
        }
        foreach (KeyValuePair<string, Effect> effect in enemyCombatData.getStatusEffects())
        {
            Debug.Log("Enemy Effect: " + effect.Key + " with count: " + effect.Value.EffectCount + " and value:" + effect.Value.EffectValue_Int);
        }
        isPlayerTurn = !isPlayerTurn;
        if (isPlayerTurn)
        {
            if (enemyCombatData.getSpiritPercentageRemaining() >= 1)
            {
                if (currentRitualSetIndex == 4)
                {
                    //do game over
                } else
                {
                    moveToNextSpiritOrb();
                }
            }
            if (playerCombatData.getSpecificStatusEffect("effect_skip") != null)
            {
                //TODO: refactor this into its own function
                playerCombatData.getSpecificStatusEffect("effect_skip").EffectCount -= 1;
                if (playerCombatData.getSpecificStatusEffect("effect_skip").EffectCount < 0)
                {
                    clearFinishedStatusEffects(playerCombatData, "effect_skip");
                }
                nextTurn();
            }
        }
    }

    private void Update()
    {
        if (isPlayerTurn == false) {
            //do enemy AI stuff here.
            if (enemyCombatData.getSpecificStatusEffect("effect_stun") != null)
            {
                //TODO: refactor this into its own function, preferably with the player skip thing.
                enemyCombatData.getSpecificStatusEffect("effect_stun").EffectCount -= 1;
                if (enemyCombatData.getSpecificStatusEffect("effect_stun").EffectCount < 0)
                {
                    clearFinishedStatusEffects(enemyCombatData, "effect_stun");
                }
                nextTurn();

            } else
            {
                

                ActionSO enemyAction = enemyAI.getAction(enemyCombatData.getHostility(), currentRitualSetIndex);
                foreach (Effect effect in enemyAction.getEffects())
                {
                    if (effect.EffectTargetsEnemy)
                    {
                        enemyCombatData.addStatusEffect(effect.EffectID, effect);
                    } else if (effect.EffectTargetsPlayer)
                    {
                        playerCombatData.addStatusEffect(effect.EffectID, effect);
                    }
                }
                executeStatusEffects(enemyCombatData.getStatusEffects(), false);
                executeStatusEffects(playerCombatData.getStatusEffects(), true);

                Debug.Log("The enemy performs " + enemyAction.getActionName());
                Debug.Log("END OF ENEMY TURN\n---------------------");
                nextTurn();
            }
        }

        if (enemyCombatData.getSpirit() >= enemyCombatData.getMaxSpirit())
        {
            //get next orb
        } else if (enemyCombatData.getHealth() <= 0 )
        {
            //finish
        } else if (playerCombatData.getHealth() <= 0)
        {
            //finish as well
        }
    }

    public void chooseRitual(int index)
    {
        chosenRitual = ritualSets[currentRitualSetIndex].getRitualSet()[index];
        runPassiveEffects(chosenRitual.getPassiveEffects());
        combatUIManager.hideRitualScreen();
        combatUIManager.updateActionButtons(getActionButtonText(chosenRitual), getActionButtonColors(chosenRitual));
    }

    private void runPassiveEffects(Effect[] passiveEffects)
    {
        foreach (Effect passiveEffect in passiveEffects)
        {
            if (passiveEffect.EffectIsPassive)
            {
                switch (passiveEffect.EffectID) {
                    case "passive_effect_origin_1":
                        passiveFlags["passiveOriginOne"] = true;
                        break;
                    case "passive_effect_calling_1":
                        passiveFlags["passiveCallingOne"] = true;
                        break;
                    case "passive_effect_calling_2":
                        passiveFlags["passiveCallingTwo"] = true;
                        passiveEffectLookupTable.Add(passiveEffects[1].EffectID, new Effect[1] { passiveEffects[1] });
                        return;
                    case "passive_effect_link_damage":
                        passiveFlags["passiveLinkDamage"] = true;
                        break;
                    case "passive_effect_journey_1":
                        passiveFlags["passiveJourneyOne"] = true;
                        break;
                    case "passive_effect_journey_3":
                        passiveFlags["passiveJourneyThree"] = true;
                        break;
                    case "passive_effect_ordeal_2":
                        passiveFlags["passiveOrdealOne"] = true;
                        break;
                }
            } else
            {
                if (passiveEffect.EffectTargetsPlayer)
                {
                    playerCombatData.addStatusEffect(passiveEffect.EffectID, passiveEffect);
                }
                if (passiveEffect.EffectTargetsEnemy)
                {
                    enemyCombatData.addStatusEffect(passiveEffect.EffectID, passiveEffect);
                }
            }
        }
    }
    private string[] getActionButtonText(RitualSO ritualSO)
    {
        string[] actionButtonText = new string[4];
        for (int i = 0; i < 4; i++)
        {
            actionButtonText[i] = ritualSO.getActionSet()[i].getActionName();
        }

        return actionButtonText;
    }

    private string[] getActionButtonColors(RitualSO ritualSO)
    {
        string[] actionButtonColors = new string[4];
        for (int i = 0; i < 4; i++)
        {
            actionButtonColors[i] = ritualSO.getActionSet()[i].getAffinity();
        }

        return actionButtonColors;
    }
}
