using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected int elementIndex;
    protected float waitTime = .5f;


    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(showTooltipOnTimer());
        //TooltipManager.getTooltipManager().showTooltip(getData(elementIndex));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        TooltipManager.getTooltipManager().hideTooltip();
    }

    //this function can be modified so that classes inheriting from Tooltip will be able to get data from CombatManager
    //also allows Tooltip to be reused in other contexts
    internal virtual string[] getData(int elementIndex)
    {
        string[] tooltipContent = new string[] { CombatManager.getInstance().getActionButtonTooltip(elementIndex),
            CombatManager.getInstance().getSpiritMeterFillPrediction(elementIndex) };
        return tooltipContent;
    }

    private void Update()
    {
        TooltipManager.getTooltipManager().setTooltipPosition(InputManager.GetInstance().GetMousePosition());
    }

    private IEnumerator showTooltipOnTimer()
    {
        yield return new WaitForSeconds(waitTime);
        string[] tooltipContent = getData(elementIndex);
        TooltipManager.getTooltipManager().showTooltip(tooltipContent[0], tooltipContent[1]);
    }
}
