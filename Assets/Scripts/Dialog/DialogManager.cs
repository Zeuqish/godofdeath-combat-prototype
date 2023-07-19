using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


// DialogManager handles showing of the dialog screen. Actually getting the dialog from the script should be handled by a separate class
// that feeds into DialogManager
public class DialogManager : MonoBehaviour
{
    private Story currentStory;
    private static DialogManager instance;
    public bool isDialogPlaying { get; private set; }

    private DialogUIManager dialogUIManager;

    //Insert variable for hooking into the dialog stuff

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one DialogManager Instance found.");
        }
        instance = this;
    }
    
    void Start()
    {
        isDialogPlaying = false;

        dialogUIManager = gameObject.GetComponent<DialogUIManager>();
        dialogUIManager.initializeDialogUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDialogPlaying)
        {
            return;
        }

        if (InputManager.GetInstance().GetInteractPressed() )
        {
            continueStory();
        }
    }

    public static DialogManager getInstance()
    {
        return instance;
    }

    public void EnterDialogMode(TextAsset storyText, int dialogType, string storySection)
    {
        currentStory = new Story(storyText.text);
        if (storySection != "")
        {
            currentStory.ChoosePathString(storySection);
        }
        isDialogPlaying = true;
        dialogUIManager.setDialogUI(dialogType);

        continueStory();
    }

    private void continueStory()
    {
        if (currentStory.canContinue)
        {
            dialogUIManager.showNext(currentStory.Continue(), currentStory);
        }
        else
        {
            exitDialogMode();
        }
    }

    private void exitDialogMode()
    {
        isDialogPlaying = false;
        dialogUIManager.deactivateDialogPanel();
    }

    public void MakeChoice(int choiceIndex)
    {
        Debug.Log("Choice Made");
        currentStory.ChooseChoiceIndex(choiceIndex);
    } 
}
