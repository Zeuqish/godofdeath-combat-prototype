using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTooltip : Tooltip
{
    internal override string[] getData(int elementIndex)
    {
        string[] tooltipContent = new string[] { CombatManager.getInstance().getItemButtonTooltip(elementIndex),
            CombatManager.getInstance().getSpiritMeterFillPrediction(elementIndex, true) };
        return tooltipContent;
    }
}
