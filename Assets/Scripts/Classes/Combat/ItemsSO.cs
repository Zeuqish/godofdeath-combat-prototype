using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemsSO : ScriptableObject
{
    [SerializeField] private Effect[] effects;
    [SerializeField] private string spiritAffinity;
    [SerializeField] private string[] itemTags;
    [SerializeField] private int capacity;
    [SerializeField] private int currentCount;
    [SerializeField] private string itemName;

    private void Awake()
    {
        currentCount = capacity;   
    }

    public void resetCount()
    {
        currentCount = capacity;
    }
    public Effect[] getEffects()
    {
        return effects;
    }
    public string getAffinity()
    {
        return spiritAffinity;
    }

    public string[] getItemTags()
    {
        return itemTags;
    }

    public string getItemName()
    {
        return itemName;
    }

    public int getCapacity()
    {
        return capacity;
    }

    public int getCurrentCount()
    {
        return currentCount;
    }

    public void reduceItemCount()
    {
        currentCount -= 1;
    }
}
