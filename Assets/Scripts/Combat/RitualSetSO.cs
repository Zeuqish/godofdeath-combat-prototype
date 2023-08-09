using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RitualSetSO : ScriptableObject
{
    [SerializeField] private RitualSO[] ritualSet = new RitualSO[3];
    [SerializeField] private string ritualSetName;
    public RitualSO[] getRitualSet() { return ritualSet; }
    public string getRitualSetName() { return ritualSetName; }

}
