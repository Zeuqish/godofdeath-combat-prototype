using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableEntity : Entity
{
    [SerializeField] private GameObject visualCue;
    public bool isPlayerInRange { set; get; }
    private void Awake()
    {
        visualCue.SetActive(false);
    }

    public void enableVisualCue()
    {
        visualCue.SetActive(true);
    }

    public void disableVisualCue()
    {
        visualCue.SetActive(false);
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
