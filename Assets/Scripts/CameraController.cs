using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 cameraOffset = new Vector3(5, 11, -14);
    // Start is called before the first frame update
    void Start()
    {

    }

    void LateUpdate()
    {
        transform.position = player.transform.position + cameraOffset;
    }
}
