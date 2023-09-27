using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    PlayerControls actions;
    
    // Statistics
    private float speed = 5f;
    private bool useFlippedSprite = false;
    [SerializeField] private GameObject unflippedSprite, flippedSprite;
    private Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

    // Cached components
    [SerializeField] GameObject animatorObject;
    private Animator animator;
    [SerializeField] I_Interactor interactor;
    Rigidbody rb;
    

    // Cached Inputs
    private Vector2 moveDir;
    void Awake(){
        instance = this;
        actions = new PlayerControls();
        actions.game.interact.performed += Interact;
    }
    void Start()
    {
        animator = animatorObject.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        interactor = GetComponent<I_Interactor>();
    }

    void FixedUpdate()
    {
        PollMoveDirection(); // Poll the moveDirection for movement here; continuous input
        rb.velocity = moveDir.x*speed*transform.right + moveDir.y*speed*transform.forward; // Apply movement to rigidbody so we can use Unity's physics system.
        /*
        if (useFlippedSprite)
        {
            flippedSprite.SetActive(true);
            unflippedSprite.SetActive(false);
        } else if (!useFlippedSprite)
        {
            flippedSprite.SetActive(false);
            unflippedSprite.SetActive(true);
        }*/

    }
    void Update(){
        UpdateAnimatorParameters(); // Update animator parameters on frame updates (when the animation would actually update)
    }
    void Interact(InputAction.CallbackContext context){
        interactor.Interact();
    }
    void PollMoveDirection(){
        moveDir = InputManager.GetInstance().GetMoveDirection();
    }
    void UpdateAnimatorParameters(){
        animator.SetFloat("XInput", moveDir.x);
        //animator.SetFloat("YInput", moveDir.y);
    }
}
