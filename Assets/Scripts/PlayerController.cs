using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    private Vector2 moveDir;
    private float speed = 5f;
    private bool useFlippedSprite = false;

    private Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    [SerializeField] GameObject animatorObject;
    private Animator animator;
    [SerializeField] private GameObject unflippedSprite, flippedSprite;
    void Awake(){
        instance = this;
    }
    void Start()
    {
        animator = animatorObject.GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        moveDir = InputManager.GetInstance().GetMoveDirection();
        if (moveDir != Vector2.zero)
        {
            if (moveDir.y > 0)
            {
                useFlippedSprite = true;
            } else if (moveDir.y < 0) {
                useFlippedSprite = false;
            }

            var isometricInput = rotationMatrix.MultiplyPoint3x4(new Vector3(moveDir.x, 0 , moveDir.y));
            animator.SetFloat("XInput", moveDir.x);
            // gameObject.transform.Translate(isometricInput * Time.deltaTime * speed);
            gameObject.transform.position += (moveDir.x*speed*Time.fixedDeltaTime)*transform.right + (moveDir.y*speed*Time.fixedDeltaTime)*transform.forward;
            animator.SetBool("isWalking", true);
        } else
        {
            animator.SetBool("isWalking", false);
        }
        
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
}
