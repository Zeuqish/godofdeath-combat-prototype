using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_Interactable : MonoBehaviour
{
    [SerializeField] private GameObject interactPrompt;
    public bool isPlayerInRange { set; get; }
    void Start()
    {
        interactPrompt.SetActive(false);
    }

    public void ShowPrompt()
    {
        interactPrompt.SetActive(true);
    }

    public void HidePrompt()
    {
        interactPrompt.SetActive(false);
    }
    public void Interact(){

    }
}
