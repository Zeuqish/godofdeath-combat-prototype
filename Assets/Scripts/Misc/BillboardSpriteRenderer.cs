using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardSpriteRenderer : MonoBehaviour
{
    Camera camera;
    void Start()
    {
        camera = Camera.main;
    }

    void LateUpdate()
    {
        transform.rotation = camera.transform.rotation;
    }
}
