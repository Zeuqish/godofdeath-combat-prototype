using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class ActionSO : ScriptableObject
{
    [SerializeField] private Effect[] effects;
    [SerializeField] private string spiritAffinity;
    [SerializeField] private string actionName;
    [SerializeField] private string flavorText;
    [SerializeField] private bool canBeReused = true;
    [SerializeField] private bool isUsedUp = false;

    public Effect[] getEffects()
    {
        return effects;
    }

    public string getAffinity()
    {
        return spiritAffinity;
    }

    public string getActionName()
    {
        return actionName;
    }

    public string getFlavorText()
    {
        return flavorText;
    }

    public bool getCanBeReused()
    {
        return canBeReused;
    }

    public bool getIsUsedUp()
    {
        return isUsedUp;
    }

    public void useUp()
    {
        isUsedUp = true;
    }
}
