using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RitualSO: ScriptableObject
{
    [SerializeField] private ActionSO[] actionSet = new ActionSO[4];
    [SerializeField] private string ritualName, passiveDescription;
    [SerializeField] private Effect[] passiveEffects;
    [SerializeField] private string[] ritualTags;

    public string getRitualName()
    {
           return ritualName;
    }

    public string getPassiveDescription()
    {
        return passiveDescription;
    }

    public ActionSO[] getActionSet()
    {
        return actionSet;
    }

    public Effect[] getPassiveEffects()
    {
        return passiveEffects;
    }

    public string[] getRitualTags()
    {
        return ritualTags;
    }
}

