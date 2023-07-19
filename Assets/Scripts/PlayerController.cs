using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveDir;
    private float speed = 5f;
    private bool useFlippedSprite = false;
    [SerializeField] private GameObject unflippedSprite, flippedSprite;
    void Start()
    {
        
    }

    void Update()
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
            gameObject.transform.Translate(new Vector3(moveDir.x, 0, moveDir.y) * Time.deltaTime * speed);
        }

        if (useFlippedSprite)
        {
            flippedSprite.SetActive(true);
            unflippedSprite.SetActive(false);
        } else if (!useFlippedSprite)
        {
            flippedSprite.SetActive(false);
            unflippedSprite.SetActive(true);
        }

    }
}
