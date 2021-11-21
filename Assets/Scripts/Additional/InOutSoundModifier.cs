using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InOutSoundModifier : MonoBehaviour
{
   
   [Header("Collider Names: _SoundIN, _SoundOUT")]
   [Space(10)]
    [SerializeField] AudioSource currentAudio;

    [Tooltip("Low - High")]
    [SerializeField] Vector2 modification_Pitch = new Vector2(0.8f, 1.2f);
    [SerializeField] Vector2 modification_Volume = new Vector2(0.8f, 1.2f);
    [SerializeField] float transaction_smoothing = 3;
    float _currentVelocity;

    void OnTriggerEnter(Collider other)
    {
        StopAllCoroutines();
        if(other.transform.gameObject.name == "_SoundIN"){
            StartCoroutine(Modification(3, false));
            //currentAudio.pitch = modification_Pitch.x;
            //currentAudio.volume = modification_Volume.x;
        }else if(other.transform.gameObject.name == "_SoundOUT"){
           // currentAudio.pitch = modification_Pitch.y;
           // currentAudio.volume = modification_Volume.y;
            StartCoroutine(Modification(3, true));
        }else return;
    }

    private IEnumerator Modification(float timer, bool inc){
            while(timer > 0){

                if(inc){
                    currentAudio.pitch = Mathf.SmoothDamp(currentAudio.pitch, modification_Pitch.y, ref _currentVelocity, transaction_smoothing);
                    currentAudio.volume = Mathf.SmoothDamp(currentAudio.volume, modification_Volume.y, ref _currentVelocity, transaction_smoothing);
                }else{
                    currentAudio.pitch = Mathf.SmoothDamp(currentAudio.pitch, modification_Pitch.x, ref _currentVelocity, transaction_smoothing);
                    currentAudio.volume = Mathf.SmoothDamp(currentAudio.volume, modification_Volume.x, ref _currentVelocity, transaction_smoothing);
                }

                timer -= Time.deltaTime;
                yield return null;
            }
    }
}
