using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private TextMeshProUGUI playerHP, enemyHP, enemySpirit;
    [SerializeField] private GameObject[] actionButtons, itemButtons, ritualButtons, spiritOrbs;
    private TextMeshProUGUI[] actionButtonsText, itemButtonsText, ritualButtonsText;
    private Image[] spiritOrbImages;

    void Awake()
    {
        EnemySpiritBar.value = 0;
        PlayerHPBar.value = 1;
        EnemyHPBar.value = 1;

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
    public void updateSpiritOrbsUI(string[] spiritOrbAffinities)
    {
        for (int i = 0; i < spiritOrbAffinities.Length; i++)
        {
            spiritOrbImages[i].color = getAffinityColor(spiritOrbAffinities[i]);
        }
    }
    
    public void updateSpiritOrbsUI(string[] spiritOrbAffinities, int[] index)
    { //method overload that updates one specific spiritOrb.

        for (int i = 0; i < index.Length; i++)
        {
            spiritOrbImages[index[i]].color = getAffinityColor(spiritOrbAffinities[i]);
        }
    }

    public void updateRitualsUI(string[] ritualNames, string[] ritualPassives)
    {
        for (int i = 0; i < 3; i++)
        {
            ritualButtonsText[i].text = ritualNames[i] + "\n" + ritualPassives[i];
        }
    }

    public void updateActionButtons(string[] buttonText, string[] buttonColors)
    {
        int index = 0;
        foreach (string text in buttonText)
        {
            actionButtonsText[index].text = text;
            Color buttonColor = getAffinityColor(buttonColors[index]);
            actionButtons[index].GetComponent<Image>().color = buttonColor;
            index++;
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
            default:
                buttonColor = new Color(1, 1, 1, 1);
                break;
        }
        return buttonColor;
    }
}
