using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildArrow : MonoBehaviour
{
    [SerializeField] float destroy_time;
    public ChildArrowProperties child_arrow_properties;

    public void Collided(){
        Invoke("DestroyNow", destroy_time);
    }

    void DestroyNow(){
        Destroy(gameObject);
    }
    
}
