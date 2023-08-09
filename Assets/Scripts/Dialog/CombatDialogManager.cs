using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatDialogManager : DialogManager
{
    [SerializeField] private TextAsset combatDialog;

    public void showCombatDialog(string combatDialogSection, string variable,  double effectCount)
    {
        Story combatDialogStory = new Story(combatDialog.text);
        combatDialogStory.variablesState[variable] = effectCount;
        EnterDialogMode(combatDialogStory, 0, combatDialogSection);
    }
    public void showCombatDialog(string combatDialogSection, string variable, int effectCount)
    {
        Story combatDialogStory = new Story(combatDialog.text);
        combatDialogStory.variablesState[variable] = effectCount;
        EnterDialogMode(combatDialogStory, 0, combatDialogSection);
    }

    public void showCombatDialog(string combatDialogSection, string variable, string valueName)
    {
        Story combatDialogStory = new Story(combatDialog.text);
        combatDialogStory.variablesState[variable] = valueName;
        EnterDialogMode(combatDialogStory, 0, combatDialogSection);
    }
    public void showCombatDialog(string combatDialogSection)
    {
        Story combatDialogStory = new Story(combatDialog.text);
        EnterDialogMode(combatDialogStory, 0, combatDialogSection);
    }

    public void createUnitSpecificDialog(bool targetsPlayer, string sectionIfPlayer, string sectionIfEnemy, string variable, int value)
    {
        if (targetsPlayer)
        {
            showCombatDialog(sectionIfPlayer, variable, value);
        } else if (!targetsPlayer)
        {
            showCombatDialog(sectionIfEnemy, variable, value);
        } else
        {
            Debug.Log("Something has gone terribly wrong");
        }
    }

    public void createUnitSpecificDialog(bool targetsPlayer, string sectionIfPlayer, string sectionIfEnemy, string variable, double value)
    {
        if (targetsPlayer)
        {
            showCombatDialog(sectionIfPlayer, variable, value);
        }
        if (!targetsPlayer)
        {
            showCombatDialog(sectionIfEnemy, variable, value);
        }
    }

    public void createUnitSpecificDialog(bool targetsPlayer, string sectionIfPlayer, string sectionIfEnemy, string variable, string value)
    {
        if (targetsPlayer)
        {
            showCombatDialog(sectionIfPlayer, variable, value);
        }
        if (!targetsPlayer)
        {
            showCombatDialog(sectionIfEnemy, variable, value);
        }
    }

    public IEnumerator checkIfDialogRunning()
    {
        while (this.isDialogPlaying)
        {
            yield return true;
        }
        
    }
}
