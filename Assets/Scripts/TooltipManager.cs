using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TooltipManager : MonoBehaviour
{
    private static TooltipManager instance;
    [SerializeField] TextMeshProUGUI tooltipText;
    [SerializeField] TextMeshProUGUI tooltipInfoText;
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] int characterWrapLimit;

    [SerializeField] GameObject tooltip;

    private void Awake()
    {
        instance = this;
        tooltip.SetActive(false);
    }

    public void setTooltipText(string text, string infoText = null)
    {
        tooltipText.text = text;

        if (infoText != null)
        {
            this.tooltipInfoText.text = "Predicted Spirit Fill: " + infoText;
        }


        int contentLength = Mathf.Max(tooltipText.text.Length, tooltipInfoText.text.Length);
        if (contentLength > characterWrapLimit)
        {
            layoutElement.enabled = true;
        }
        else
        {
            layoutElement.enabled = false;
        }

    }

    public void setTooltipPosition(Vector2 position)
    {
        tooltip.transform.position = position;
    }
    public void showTooltip(string tooltipText, string tooltipInfoText = null)
    {
        setTooltipText(tooltipText, tooltipInfoText);
        tooltip.gameObject.SetActive(true);
    }

    public void hideTooltip()
    {
        tooltip.gameObject.SetActive(false);
    }

    public static TooltipManager getTooltipManager()
    {
        return instance;
    }


}
