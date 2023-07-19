using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogUIManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI dialogText;

    [SerializeField] private GameObject[] choicesUIElements;

    private TextMeshProUGUI[] choicesUIElements_Text;
    public void initializeDialogUI()
    {
        dialogPanel.SetActive(false);
        choicesUIElements_Text = new TextMeshProUGUI[choicesUIElements.Length];
        int index = 0;
        foreach (GameObject choice in choicesUIElements)
        {
            choicesUIElements_Text[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }
    public void setDialogUI(int dialogType) {
        activateDialogPanel();
        switch (dialogType)
        {
            case 0: //in-game dialog
                break; 
            case 1: //interaction dialog
                break;
            case 2: //cutscene dialog
                break;
        }
    }

    public void activateDialogPanel()
    {
        dialogPanel.SetActive(true);
    }

    public void deactivateDialogPanel()
    {
        dialogPanel.SetActive(false);
        dialogText.text = "";
    }

    public void setDialogText(string storyText)
    {
        dialogText.text = storyText;
    }

    public void showNext(string storyText, Story currentStory)
    {
        setDialogText(storyText);
        displayChoices(currentStory);
    }

    public void displayChoices(Story currentStory)
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choicesUIElements.Length)
        {
            Debug.LogError("More choices given than UI can support");
        }

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            choicesUIElements[index].gameObject.SetActive(true);
            choicesUIElements_Text[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choicesUIElements.Length; i++)
        {
            choicesUIElements[i].gameObject.SetActive(false);
        }

        //StartCoroutine(selectFirstChoice());
    }

    //workaround function to select a first choice from the choices given.
    //TODO refactor into the proper class
    private IEnumerator selectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choicesUIElements[0].gameObject);
    }


}
