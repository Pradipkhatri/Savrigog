using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoFall : MonoBehaviour
{
    [SerializeField] private float holdTime;

    [SerializeField] Rigidbody rb;

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag != "Player") return;
        GetComponent<SphereCollider>().enabled = false;
        GameManager.gameManager.ShakeCamera(0.2f, holdTime, transform.position);
        Invoke("Fall", holdTime);
    }

    void Fall(){
        rb.isKinematic = false; 
        Invoke("Destroy", 10);
    }

    void Destroy(){
        Destroy(rb.gameObject);
    }
}
