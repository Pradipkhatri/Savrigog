using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAudioTimer : MonoBehaviour
{
    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.time = Random.Range(0, audioSource.clip.length);
    }

  
}
