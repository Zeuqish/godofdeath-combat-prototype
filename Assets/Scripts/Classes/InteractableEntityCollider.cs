using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableEntityCollider : MonoBehaviour
{
    private InteractableEntity parentInteractableEntity;
    private void Awake()
    {
        parentInteractableEntity = gameObject.GetComponentInParent<InteractableEntity>();
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            parentInteractableEntity.isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            parentInteractableEntity.isPlayerInRange = false;
        }
    }
}
