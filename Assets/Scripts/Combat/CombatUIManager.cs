using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIManager : MonoBehaviour
{
    public Slider EnemySpiritBar;
    public Slider PlayerHPBar;
    public Slider EnemyHPBar;
    public GameObject ritualSelectionScreen;
    public GameObject mainCombatScreen;

    [SerializeField] private TextMeshProUGUI playerHP, enemyHP, enemySpirit, hostilityValue, playerStatusEffects, enemyStatusEffects;
    [SerializeField] private GameObject[] actionButtons, itemButtons, ritualButtons, spiritOrbs;
    private TextMeshProUGUI[] actionButtonsText, itemButtonsText, ritualButtonsText;
    private Image[] spiritOrbImages;

    private string[] statusEffectsToShow = new string[6] {"effect_stun","effect_defend", "effect_charge", "effect_dodge", "effect_charge", "effect_item_disable"};
    void Awake()
    {
        EnemySpiritBar.value = 0;
        PlayerHPBar.value = 1;
        EnemyHPBar.value = 1;
        hostilityValue.text = 0.ToString();
        playerStatusEffects.text = "";
        enemyStatusEffects.text = "";

        initializeUI();
    }

    void initializeUI()
    {
        actionButtonsText = new TextMeshProUGUI[actionButtons.Length];
        itemButtonsText = new TextMeshProUGUI[itemButtons.Length];
        ritualButtonsText = new TextMeshProUGUI[ritualButtons.Length];
        spiritOrbImages = new Image[spiritOrbs.Length];

        int index = 0;
        foreach (GameObject button in actionButtons)
        {
            actionButtonsText[index] = button.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
        
        index = 0;
        foreach (GameObject button in itemButtons)
        {
            itemButtonsText[index] = button.GetComponentInChildren<TextMeshProUGUI>();
            itemButtons[index].SetActive(false);
            index++;
        }

        index = 0;
        foreach (GameObject button in ritualButtons)
        {
            ritualButtonsText[index] = button.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }

        index = 0;
        foreach (GameObject spiritOrb in spiritOrbs)
        {
            spiritOrbImages[index] = spiritOrb.GetComponentInChildren<Image>();
            index++;
        }

    }

    public void showRitualScreen()
    {
        ritualSelectionScreen.SetActive(true);
        mainCombatScreen.SetActive(false);
    }

    public void hideRitualScreen()
    {
        ritualSelectionScreen.SetActive(false);
        mainCombatScreen.SetActive(true);
    }

    public void disableActionButton(int index)
    {
        actionButtons[index].GetComponent<Button>().enabled = false;
    }

    public void enableActionButton(int index)
    {
        actionButtons[index].GetComponent<Button>().enabled = true;
    }

    public void disableItemButton(int index)
    {
        itemButtons[index].GetComponent<Button>().enabled = false;
    }

    public void enableItemButton (int index)
    {
        itemButtons[index ].GetComponent<Button>().enabled = true;
    }
    public void updateSpiritBar(float newValue)
    {
        EnemySpiritBar.value = newValue;
    }

    public void updatePlayerHPBar(float newValue)
    {
        PlayerHPBar.value = newValue;
    }
    public void updateEnemyHPBar(float newValue)
    {
        EnemyHPBar.value= newValue;
    }

    public void updatePlayerHP(int newValue)
    {
        playerHP.text = newValue.ToString();
    }

    public void updateEnemyHP(int newValue)
    {
        enemyHP.text = newValue.ToString();
    }

    public void updateEnemySpirit(int newValue)
    {
        enemySpirit.text = newValue.ToString();
    }

    public void updateHostilityValue(int newValue)
    {
        hostilityValue.text = newValue.ToString();
    }
    public void updateSpiritOrbsUI(string[] spiritOrbAffinities)
    {
        for (int i = 0; i < spiritOrbAffinities.Length; i++)
        {
            spiritOrbImages[i].color = getAffinityColor(spiritOrbAffinities[i]);
        }
    }
    
    public void updateSpiritOrbsUI(string[] spiritOrbAffinities, int[] index)
    { //method overload that updates specific spiritOrbs.

        for (int i = 0; i < index.Length; i++)
        {
            spiritOrbImages[index[i]].color = getAffinityColor(spiritOrbAffinities[i]);
        }
    }

    public void updateRitualsUI(string[] ritualNames, string[] ritualPassives)
    {
        for (int i = 0; i < ritualNames.Length; i++)
        {
            ritualButtons[i].SetActive(true);
            ritualButtonsText[i].text = ritualNames[i] + "\n" + ritualPassives[i];
        }
        if (ritualNames.Length < 3)
        {
            for (int i = ritualNames.Length; i < 3; i++)
            {
                ritualButtons[i].SetActive(false);
            }
        }
    }

    public void updateActionButtons(string[] buttonText, string[] buttonColors)
    {
        int index = 0;
        foreach (string text in buttonText)
        {
            actionButtons[index].SetActive(true);
            actionButtonsText[index].text = text;
            Color buttonColor = getAffinityColor(buttonColors[index]);
            actionButtons[index].GetComponent<Image>().color = buttonColor;
            index++;
        }

        if (buttonText.Length < 4)
        {
            for (int i = buttonText.Length; i < 4; i++)
            {
                actionButtons[i].SetActive(false);
            }
        }
    }

    public void updateItemButtons(string[] buttonText, string[] buttonColors)
    {
        int index = 0;
        foreach (string text in buttonText)
        {
            itemButtonsText[index].text = text;
            Color buttonColor = getAffinityColor(buttonColors[index]);
            itemButtons[index].GetComponent<Image>().color = buttonColor;
            itemButtons[index].SetActive(true);
            index++;
        }
    }

    public void updatePlayerStatusEffects(Dictionary<string, Effect>.ValueCollection effectsList)
    {
        string[] statusEffects = new string[6] {"", "", "", "", "", ""};
        int index = 0;
        foreach (Effect effect in effectsList)
        {
            if (statusEffectsToShow.Any(e => effect.EffectID.Contains(e)))
            {
                string status_effect = "";
                switch (effect.EffectID)
                {
                    case "effect_defend":
                        status_effect = "Defend - Reduce Damage Taken by 50%";
                        break;
                    case "effect_charge":
                        status_effect = "Charge - Increase Damage by 50%";
                        break;
                    case "effect_vulnerable":
                        status_effect = "Vulnerable - Increase Damage Taken by 50%";
                        break;
                    case "effect_dodge":
                        status_effect = "Dodge - Ignore Damage";
                        break;
                    case "effect_stun":
                        status_effect = "Stun - Turn Skipped";
                        break;
                    case "effect_item_disable":
                        status_effect = "Item Locked";
                        break;
                }
                status_effect = status_effect + " (" + (effect.EffectCount + 1).ToString() + ")";
                statusEffects[index] = status_effect;
                index++;
            }
        }
        playerStatusEffects.text = String.Join("\n", statusEffects);
    }

    public void updateEnemyStatusEffects(Dictionary<string, Effect>.ValueCollection effectsList)
    {
        string[] statusEffects = new string[6] { "", "", "", "", "", "" };
        int index = 0;
        foreach (Effect effect in effectsList)
        {
            if (statusEffectsToShow.Any(e => effect.EffectID.Contains(e)))
            {
                string status_effect = "";
                switch (effect.EffectID)
                {
                    case "effect_defend":
                        status_effect = "Defend - Reduce Damage Taken by 50%";
                        break;
                    case "effect_charge":
                        status_effect = "Charge - Increase Damage by 100%";
                        break;
                    case "effect_vulnerable":
                        status_effect = "Vulnerable - Increase Damage Taken by 50%";
                        break;
                    case "effect_dodge":
                        status_effect = "Dodge - Ignore Damage";
                        break;
                    case "effect_stun":
                        status_effect = "Stun - Turn Skipped";
                        break;
                    case "effect_item_disable":
                        status_effect = "Item Locked";
                        break;
                }
                status_effect = status_effect + " (" + (effect.EffectCount + 1).ToString() + ")";
                statusEffects[index] = status_effect;
                index++;
            }
        }
        playerStatusEffects.text = String.Join("\n", statusEffects);
    }
    public Color getAffinityColor(string color)
    {
        Color buttonColor;
        switch (color)
        {
            case "affinity_red":
                buttonColor = new Color(.678f, .204f, .243f, 1);
                break;
            case "affinity_teal":
                buttonColor = new Color(.655f, 1, .965f, 1);
                break;
            case "affinity_white":
                buttonColor = new Color(.961f, .961f, .961f, 1);
                break;
            case "affinity_black":
                buttonColor = new Color(.157f, .157f, .165f, 1);
                break;
            case "affinity_green":
                buttonColor = new Color(.118f, .659f, .588f, 1);
                break;
            case "affinity_hidden":
                buttonColor = new Color(.263f, .498f, .592f, 1);
                break;
            default:
                buttonColor = new Color(1, 1, 1, 1);
                break;
        }
        return buttonColor;
    }
}
