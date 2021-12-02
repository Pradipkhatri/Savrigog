using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAudioPlayer : MonoBehaviour, ITrigger
{
    
    [SerializeField] AudioSource audioSource;
    [SerializeField] Transform audioLocation;
    [SerializeField] AudioClip audioClip;

    bool hasPlayed = false;

    public void TriggerEnter(){
        if(!hasPlayed){
            audioSource.transform.position = audioLocation.position;
            audioSource.PlayOneShot(audioClip);
            hasPlayed = true;
        }else return;
    }
}
