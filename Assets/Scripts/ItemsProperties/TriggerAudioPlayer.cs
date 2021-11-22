using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAudioPlayer : MonoBehaviour, ITrigger
{
    public void TriggerEnter(){
        Debug.Log("Entered Trigger");
    }
}
