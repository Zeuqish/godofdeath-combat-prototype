using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class CombatManager : MonoBehaviour
{
    [SerializeField] private RitualSetSO[] ritualSets = new RitualSetSO[5];
    [SerializeField] private ItemsSO[] items = new ItemsSO[3];
    [SerializeField] private RitualManager ritualManager;
    [SerializeField] private bool isPlayerTurn = true;

    private int currentRitualSetIndex = 0;
    private RitualSO chosenRitual;

    private bool actionMatchesAffinity = false;
    private int spiritFillBuffer = 0;
    private static CombatManager instance;

    [SerializeField] private CombatUIManager combatUIManager;
    [SerializeField] private EnemyAISO enemyAI;
    [SerializeField] private CombatDataSO playerCombatData;
    [SerializeField] private CombatDataSO enemyCombatData;
    [SerializeField] private CombatDialogManager combatDialogManager;
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

    [SerializeField] private OrbSO[] enemySpiritOrbs;

    private bool playerCurrentlyChoosingRitual = false; //hack

    private void Awake()
    {
        playerCombatData.Awake();
        enemyCombatData.Awake();
        enemyCombatData.setSpiritAffinities(enemySpiritOrbs);

        setRitualChoices();
        setItemChoices();
        combatUIManager.updateSpiritOrbsUI(enemyCombatData.getSpiritAffinities());
        combatUIManager.updateHostilityValue(enemyCombatData.getHostility());
        instance = this;
        
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
        Debug.Log(currentRitualSetIndex.ToString() + ritualSets[currentRitualSetIndex].getRitualSetName());
        string[] ritualNames = new string[currentRitualSet.Length];
        string[] ritualPassivesNames = new string[currentRitualSet.Length];
        for (int i = 0; i < currentRitualSet.Length; i++)
        {
            ritualNames[i] = currentRitualSet[i].getRitualName();
            ritualPassivesNames[i] = currentRitualSet[i].getPassiveDescription();
        }

        combatUIManager.updateRitualsUI(ritualNames, ritualPassivesNames);
    }
    public void makeAction(int actionIndex)
    {
        if (CombatDialogManager.getInstance().isDialogPlaying)
        {
            return;
        } else
        {
            StartCoroutine(doAction(actionIndex));
        }
        
    }

    private IEnumerator doAction(int actionIndex)
    {
        ActionSO chosenAction = chosenRitual.getActionSet()[actionIndex];
        if (!chosenAction.getCanBeReused())
        {
            if (chosenAction.getIsUsedUp())
            {
                yield return null;
            }
            else
            {
                combatUIManager.disableActionButton(actionIndex);
                chosenAction.useUp();
            }
        }
        //pushing operation
        foreach (Effect effect in chosenAction.getEffects())
        {
            if (effect.EffectTargetsEnemy)
            {
                enemyCombatData.addStatusEffect(effect.EffectID, effect);
            }
            else if (effect.EffectTargetsPlayer)
            {
                playerCombatData.addStatusEffect(effect.EffectID, effect);
            }
        }

        spiritFillBuffer += processAffinities(chosenAction.getAffinity(), true); //spiritFillBuffer keeps track of all pre-modified spirit meter fill
        yield return (executeStatusEffectsInOrder());

        //StartCoroutine(executeStatusEffects(playerCombatData.getStatusEffects(), true));
        //StartCoroutine(executeStatusEffects(enemyCombatData.getStatusEffects(), false));

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
        StartCoroutine(nextTurn()); //this next turn always goes to enemy turn.
    }
    public void useItem(int itemIndex)
    {
        ItemsSO chosenItem = items[itemIndex];
        if (playerCombatData.getIfItemsDisabled())
        {
            return;
        }

        if (!playerCombatData.reduceCount("effect_item_free"))
        {
            if (chosenItem.getCurrentCount() > 0)
            {
                chosenItem.reduceItemCount();
                if (chosenItem.getCurrentCount() <= 0)
                {
                    combatUIManager.disableItemButton(itemIndex);
                }
            } else
            {
                return;
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
        StartCoroutine(executeStatusEffects(playerCombatData.getStatusEffects()));
        StartCoroutine(executeStatusEffects(enemyCombatData.getStatusEffects()));

        spiritFillBuffer += processTags(chosenItem.getItemTags());
        spiritFillBuffer += processAffinities(chosenItem.getAffinity(), false);
       
        fillSpiritMeter(spiritFillBuffer);

        StartCoroutine(nextTurn());
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
            target.reduceCount("effect_defend");
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
        playerCurrentlyChoosingRitual = true;
        currentRitualSetIndex += 1;
        if (currentRitualSetIndex == 5)
        {
            combatDialogManager.showCombatDialog("prototype_done");
            currentRitualSetIndex = 4;
        }
        //Debug.Log(ritualSets[currentRitualSetIndex].getRitualSetName());
        enemyCombatData.resetSpiritMeter();
        fillSpiritMeter(0); //shorthand for resetting the spirit meter
        setRitualChoices();
        combatUIManager.showRitualScreen();
    }

    private IEnumerator runEffect(Effect effect)
    {
        //Debug.Log("running effect" + effect.EffectID);
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
                int passive_journey_three_damage = 0;
                //Debug.Log("damage is being cast: " + caster.sendDamage(effect.EffectValue_Int).ToString() + " by " + caster.ToString());
                combatDialogManager.createUnitSpecificDialog(effect.EffectTargetsPlayer, "player_injured", "enemy_injured", "effect_count", target.calculateDamageTaken(caster.sendDamage(effect.EffectValue_Int)));
               if (passiveFlags["passiveJourneyThree"])
                {
                    passive_journey_three_damage += 5;
                }
                
                doDamage(caster.sendDamage(effect.EffectValue_Int + passive_journey_three_damage), target);
                break;
            case "effect_defend":
                combatDialogManager.createUnitSpecificDialog(effect.EffectTargetsPlayer, "player_status_effect_applied", "enemy_status_effect_applied", "status_effect", "Defending");
                target.setDamageTakenModifier((float) .5);
                break;
            case "effect_defense_reduce":

                target.setPassiveDamageTakenModifier(effect.EffectValue_Float);
                break;
            case "effect_destroy_orb":
                Debug.Log("destroying orb");
                fillSpiritMeter(100);
                break;
            case "effect_dodge":
                combatDialogManager.createUnitSpecificDialog(effect.EffectTargetsPlayer, "player_status_effect_applied", "enemy_status_effect_applied", "status_effect", "Dodging");
                target.setDamageTakenModifier((float)0);
                break;
            case "effect_flat_damage_bonus":
                target.setFlatDamageBonusModifier(effect.EffectValue_Int);
                break;
            case "effect_heal":
                combatDialogManager.createUnitSpecificDialog(effect.EffectTargetsPlayer, "player_healed", "enemy_healed", "effect_count", effect.EffectValue_Int);
                target.increaseHealth(effect.EffectValue_Int);
                break;
            case "effect_hostility_add":
                combatDialogManager.showCombatDialog("enemy_hostility_increased", "effect_count", effect.EffectValue_Int);
                enemyCombatData.increaseHostility(effect.EffectValue_Int);
                combatUIManager.updateHostilityValue(enemyCombatData.getHostility());
                break;
            case "effect_hostility_reduce":
                combatDialogManager.showCombatDialog("enemy_hostility_decreased", "effect_count", effect.EffectValue_Int);
                enemyCombatData.decreaseHostility(effect.EffectValue_Int);
                combatUIManager.updateHostilityValue(enemyCombatData.getHostility());
                break;
            case "effect_item_free":
                break;
            case "effect_item_disable":
                playerCombatData.disableItemUse();
                break;
            case "effect_increase_spirit_meter":
                combatDialogManager.showCombatDialog("enemy_increased_spirit_meter", "effect_count", effect.EffectValue_Int);
                enemyCombatData.increaseMaximumSpiritBar(effect.EffectValue_Int);
                break;
            case "effect_spirit_fill":
                spiritFillBuffer += effect.EffectValue_Int;

                if (passiveFlags["passiveJourneyThree"])
                {
                    spiritFillBuffer += 10;
                }
                break;
            case "effect_spirit_fill_mult":
                // this is only used by the 
                if (actionMatchesAffinity)
                {
                    enemyCombatData.setSpiritMeterFillModifier(effect.EffectValue_Int);
                }
                break;
            case "effect_vulnerable":
                target.setVulnerableDamageTakenModifier((float)2);
                break;
            case "effect_none":
                combatDialogManager.showCombatDialog("enemy_do_nothing");
                break;
        }
        yield return null;
    }

    private IEnumerator executeStatusEffects(Dictionary<string, Effect> effects)
    {
        string[] effectIDs = new string[] {
            "effect_destroy_orb",
            "effect_item_free",
            "effect_item_disable",
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
                //Debug.Log(effectID);
                // effects that aren't per instance of damage get procced. otherwise, they are procced via doDamage().
                if (effect.EffectCount < 0)
                {
                    finishedEffects.Add(effectID);
                    continue;
                }
                
                if (effectID != "effect_defend" && effectID != "effect_dodge" && effectID != "effect_charge" && effectID != "effect_vulnerable")
                {
                    effect.EffectCount -= 1;
                }
                

                yield return runEffect(effect);

                if (effect.EffectCount < 0)
                {
                    //push effect into effect list of things to remove
                    finishedEffects.Add(effectID);
                }
            }
            yield return combatDialogManager.checkIfDialogRunning();
        }
        clearFinishedStatusEffects(playerCombatData, finishedEffects);
        clearFinishedStatusEffects(enemyCombatData, finishedEffects);
        
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
        int spiritFill = playerCombatData.getBaseSpiritFill();
        if (isAction)
        {
            spiritFill += 10;
        }

        OrbSO currentOrb = enemyCombatData.getSpiritOrbs()[currentRitualSetIndex];
        if (affinity == currentOrb.getOrbAffinity() || (affinity == currentOrb.getRawOrbAffinity() && !currentOrb.isOrbVisible()))
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
            if (!actionMatchesAffinity)
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
    IEnumerator nextTurn ()
    {
        combatUIManager.updatePlayerStatusEffects(playerCombatData.getStatusEffects().Values);
        isPlayerTurn = !isPlayerTurn;

        if (actionMatchesAffinity && !enemyCombatData.getSpiritOrbs()[currentRitualSetIndex].isOrbVisible() && enemyCombatData.getSpiritOrbs()[currentRitualSetIndex].isOrbReadable())
        {
            Debug.Log("Orb should be visible");
            enemyCombatData.getSpiritOrbs()[currentRitualSetIndex].isOrbVisible();
            combatUIManager.updateSpiritOrbsUI(new string[1] { enemyCombatData.getSpiritOrbs()[currentRitualSetIndex].getRawOrbAffinity() },
                new int[1] {currentRitualSetIndex});
        }

        if (isPlayerTurn)
        {
           
        } else if (isPlayerTurn == false)
        {
            yield return StartCoroutine(enemyTurn());
        }
    }

    IEnumerator enemyTurn()
    {
        //do enemy AI stuff here.
        if (enemyCombatData.getSpecificStatusEffect("effect_stun") != null)
        {
            //TODO: refactor this into its own function, preferably with the player skip thing.
            combatDialogManager.showCombatDialog("enemy_stunned");
            enemyCombatData.getSpecificStatusEffect("effect_stun").EffectCount -= 1;
            if (enemyCombatData.getSpecificStatusEffect("effect_stun").EffectCount < 0)
            {
                if (passiveFlags["passiveCallingTwo"])
                {
                    foreach (Effect effect in passiveEffectLookupTable["passiveCallingTwo"])
                    {
                        enemyCombatData.addStatusEffect(effect.EffectID, effect);
                    }
                }
                clearFinishedStatusEffects(enemyCombatData, "effect_stun");
            }

            StartCoroutine(nextTurn());
        }
        else
        {

            Debug.Log("START OF ENEMY TURN");
            ActionSO enemyAction = enemyAI.getAction(enemyCombatData.getHostility(), currentRitualSetIndex);
            
            foreach (Effect effect in enemyAction.getEffects())
            {
                if (effect.EffectTargetsEnemy)
                {
                    enemyCombatData.addStatusEffect(effect.EffectID, effect);
                }
                else if (effect.EffectTargetsPlayer)
                {
                    playerCombatData.addStatusEffect(effect.EffectID, effect);
                }
            }
            combatDialogManager.showCombatDialog("enemy_action", "name", enemyAction.getActionName());
            yield return executeStatusEffectsInOrder();
            //yield return executeStatusEffects(playerCombatData.getStatusEffects(), true);
            //yield return executeStatusEffects(enemyCombatData.getStatusEffects(), false);

            
            Debug.Log("The enemy performs " + enemyAction.getActionName());
            Debug.Log("END OF ENEMY TURN\n---------------------");
            StartCoroutine(nextTurn());
        }

    }
    private void Update()
    {

        if (isPlayerTurn)
        {
            if (enemyCombatData.getSpiritPercentageRemaining() >= 1 && !CombatDialogManager.getInstance().isDialogPlaying)
            {
                if (currentRitualSetIndex == 5)
                {
                    combatDialogManager.showCombatDialog("prototype_done");
                    currentRitualSetIndex = 4;
                }
                else
                {
                    moveToNextSpiritOrb();
                }
            }
            if (playerCombatData.getSpecificStatusEffect("effect_skip") != null && !playerCurrentlyChoosingRitual)
            {
                //TODO: refactor this into its own function
                combatDialogManager.showCombatDialog("player_stunned");
                playerCombatData.getSpecificStatusEffect("effect_skip").EffectCount -= 1;
                if (playerCombatData.getSpecificStatusEffect("effect_skip").EffectCount < 0)
                {
                    clearFinishedStatusEffects(playerCombatData, "effect_skip");
                }
                StartCoroutine(nextTurn());
            }
        }
    }
    public void chooseRitual(int index)
    {
        chosenRitual = ritualSets[currentRitualSetIndex].getRitualSet()[index];
        runPassiveEffects(chosenRitual.getPassiveEffects());
        combatUIManager.hideRitualScreen();
        combatUIManager.updateActionButtons(getActionButtonText(chosenRitual), getActionButtonColors(chosenRitual));
        playerCurrentlyChoosingRitual = false;
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
                        passiveEffectLookupTable.Add("passiveCallingTwo", new Effect[2] { passiveEffects[1], passiveEffects[2] });
                        return;
                    case "passive_effect_link_damage":
                        passiveFlags["passiveLinkDamage"] = true;
                        break;
                    case "passive_effect_journey_1":
                        passiveFlags["passiveJourneyOne"] = true;
                        break;
                    case "passive_effect_journey_3":
                        passiveFlags["passiveJourneyThree"] = true;
                        enemyCombatData.getSpiritOrbs()[2].setIsReadable(false);
                        enemyCombatData.getSpiritOrbs()[3].setIsReadable(false);
                        break;
                    case "passive_effect_ordeal_2":
                        passiveFlags["passiveOrdealTwo"] = true;
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
        string[] actionButtonText = new string[ritualSO.getActionSet().Length];
        for (int i = 0; i < ritualSO.getActionSet().Length; i++)
        {
            actionButtonText[i] = ritualSO.getActionSet()[i].getActionName();
        }

        return actionButtonText;
    }

    private string[] getActionButtonColors(RitualSO ritualSO)
    {
        int numOfActions = ritualSO.getActionSet().Length;
        string[] actionButtonColors = new string[numOfActions];
        for (int i = 0; i < numOfActions; i++)
        {
            actionButtonColors[i] = ritualSO.getActionSet()[i].getAffinity();
        }

        return actionButtonColors;
    }
    
    public string getSpiritMeterFillPrediction(int elementIndex, bool isItem = false)
    {

        int min = 0;
        if (isItem)
        {
            min += processAffinities(items[elementIndex].getAffinity(), false);
            min += processTags(items[elementIndex].getItemTags());
        } else
        {
            min += processAffinities(chosenRitual.getActionSet()[elementIndex].getAffinity(), true);
        }
        int max = min * 2;

        if (passiveFlags["passiveCallingOne"])
        {
            if (actionMatchesAffinity)
            {
                min -= 10;
                max = (min * 2) + 10;
            }
        }
       
        if (passiveFlags["passiveOriginOne"])
        {
            max += 20;
        }

        if (passiveFlags["passiveJourneyOne"])
        {
            max += 10;
        }
        actionMatchesAffinity = false;
        
        return min.ToString() + " - " + max.ToString();
    }

    public string getActionButtonTooltip(int buttonIndex)
    { 
        return chosenRitual.getActionSet()[buttonIndex].getFlavorText();
    }

    public string getItemButtonTooltip(int buttonIndex)
    {
        StringBuilder output = new StringBuilder(100);
        output.Append("Tags: ");
        foreach (String tag in items[buttonIndex].getItemTags())
        {
            switch(tag)
            {
                case "tag_rotting":
                    output.Append("Rotting");
                    break;
                case "tag_handy":
                    output.Append("Handy");
                    break;
                case "tag_otherworldly":
                    output.Append("Otherworldly");
                    break;
                case "tag_sacred":
                    output.Append("Sacred");
                    break;
                case "tag_magic":
                    output.Append("Magic");
                    break;
                case "tag_mundane":
                    output.Append("Mundane");
                    break;
            }
            output.Append(", ");
        }; 
           
        output.AppendLine(items[buttonIndex].getEffectText());
        return output.ToString();
    }
    public static CombatManager getInstance()
    {
        return instance;
    }

    private IEnumerator executeStatusEffectsInOrder()
    {
        yield return executeStatusEffects(playerCombatData.getStatusEffects());
        yield return executeStatusEffects(enemyCombatData.getStatusEffects());
    }
}
