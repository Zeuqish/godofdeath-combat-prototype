using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script used by agents that are capable of interacting with the environment
public class I_Interactor : MonoBehaviour
{
    private List<I_Interactable> contactedInteractables;
    int interactionTarget;
    [SerializeField] private int InteractionTarget{
        get{return interactionTarget;} 
        set {interactionTarget = (value > 0) ? value: 0;} // clamp the interaction target to 0 or positive
        }
    void Start(){
        contactedInteractables = new List<I_Interactable>();
    }
    public void FindInteractables(){ // Find by proximity

    }
    public void OnInteractableEnter(I_Interactable other){
        contactedInteractables.Add(other);
        SetTarget(contactedInteractables.Count-1);
    }
    public void OnInteractableExit(I_Interactable other){
        contactedInteractables.Remove(other);
        other.HidePrompt();
        SetTarget(contactedInteractables.Count-1);
    }
    public void SetTarget(int index){
        if(InteractionTarget >= contactedInteractables.Count || index >= contactedInteractables.Count) return;
        contactedInteractables[InteractionTarget].HidePrompt();
        contactedInteractables[index].ShowPrompt();
        InteractionTarget = index;
    }
    public void Interact(){
        InteractWith(InteractionTarget);
    }
    public void InteractWith(int index){
        if(index >= contactedInteractables.Count-1) return; // Fail interacting if nothing to interact with
        contactedInteractables[index].Interact();
    }

    void OnTriggerEnter(Collider other){
        if(other.TryGetComponent<I_Interactable>(out I_Interactable interactable)){
            OnInteractableEnter(interactable);
        }
    }
    void OnTriggerExit(Collider other){
        if(other.TryGetComponent<I_Interactable>(out I_Interactable interactable)){
            OnInteractableExit(interactable);
        }
    }
}
