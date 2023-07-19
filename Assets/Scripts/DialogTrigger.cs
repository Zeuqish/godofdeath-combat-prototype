using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private TextAsset storyDialog;
    public int dialogType = 0;
    public string storySection = "";
    private InteractableEntity parentInteractableEntity;
    private void Awake()
    {
        parentInteractableEntity = gameObject.GetComponentInParent<InteractableEntity>();
        parentInteractableEntity.isPlayerInRange = false;
    }

    private void Update()
    {
        if (parentInteractableEntity.isPlayerInRange && !DialogManager.getInstance().isDialogPlaying) {
            parentInteractableEntity.enableVisualCue();
            if (InputManager.GetInstance().GetInteractPressed())
            {
                DialogManager.getInstance().EnterDialogMode(storyDialog, dialogType, storySection);
            }
        } else
        {
            parentInteractableEntity.disableVisualCue();
        }
    }
}
