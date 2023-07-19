using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
    private InteractableEntity parentInteractableEntity;

    private void Awake()
    {
        parentInteractableEntity = gameObject.GetComponentInParent<InteractableEntity>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
