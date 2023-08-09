using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RitualManager : MonoBehaviour
{
    private void Awake()
    {
        
    }
    public RitualSO[] getRitualDetails(RitualSetSO ritualSet)
    {
        return ritualSet.getRitualSet();
    }

}
