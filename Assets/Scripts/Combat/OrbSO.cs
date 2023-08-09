using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class OrbSO: ScriptableObject
{
    [SerializeField] private bool isReadable = true;
    [SerializeField] private bool isVisible = false;
    [SerializeField] private string affinity;

    public bool isOrbReadable()
    {
        return isReadable;
    }

    public bool isOrbVisible()
    {
        return isVisible;
    }

    public string getRawOrbAffinity()
    {
        return affinity;
    }

    public string getOrbAffinity()
    {
        if (!isVisible)
        {
            return "affinity_hidden";
        }
        return affinity;
    }

    public void setIsReadable(bool value)
    {
        isReadable = value;
    }

    public void setIsVisible(bool value)
    {
        isVisible = value;
    }
}
