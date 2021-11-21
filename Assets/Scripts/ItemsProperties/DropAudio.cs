using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropAudio : MonoBehaviour
{
    [SerializeField] AudioClip contact_audio;

    void OnCollisionEnter(Collision other){
        AudioCaller.PlaySound(GameManager.gameManager.secoandary_audioSource, contact_audio);
    }
    
}
