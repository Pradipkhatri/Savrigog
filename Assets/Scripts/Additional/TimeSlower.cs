using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSlower : MonoBehaviour
{
    
    [SerializeField] float slow_Time;
    [SerializeField] bool isSlowed;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T)){
            if(!isSlowed){
                GameManager.gameManager.StartCoroutine(GameManager.gameManager.SlowMotionTrigger(2));
                isSlowed = true;
            }else{
                GameManager.gameManager.StartCoroutine(GameManager.gameManager.SlowMotionTrigger(2));
                isSlowed = false;
            }
        }
    }
}
