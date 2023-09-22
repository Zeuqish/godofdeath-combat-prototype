using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConfineSwitcher : MonoBehaviour
{
    [SerializeField]GameObject vcam;
    void Start(){
        if(vcam == null) return;
        vcam.SetActive(false);
    }
    void OnTriggerEnter(Collider other){
        if(other.gameObject != PlayerController.instance.gameObject) return;
        vcam.SetActive(true);
    }
    void OnTriggerExit(Collider other){
        if(other.gameObject != PlayerController.instance.gameObject) return;
        vcam.SetActive(false);
    }
}
