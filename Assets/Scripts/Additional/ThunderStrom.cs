using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderStrom : MonoBehaviour
{
    [SerializeField] Vector2 thunderMaxTimes = new Vector2(2, 5);
    float current_Time;
    [SerializeField] Vector2 thunderStay_Time = new Vector2(0.2f, 0.4f);
    [SerializeField] int thunder_Intensity;

    [SerializeField] GameObject thunderLight;
    Light thunder_light;
    [SerializeField] AudioSource rain_audioSource;
    [SerializeField] AudioClip[] strikeAudios;

    bool striked = false;
   
    void Start()
    {
        thunder_light = thunderLight.GetComponent<Light>();
        thunderLight.SetActive(false);
        current_Time = thunderMaxTimes.x;
    }

    void Update()
    {
        if(current_Time > 0){
            striked = false;
            StopAllCoroutines();
            thunderLight.SetActive(true);
            if(thunder_light.intensity > 0) thunder_light.intensity -= Time.deltaTime * 8;
            current_Time -= Time.deltaTime;
        }else{           
            StartCoroutine(Strike());
        }

    }

    private IEnumerator Strike(){
        
        if(!striked) {
            bool play_audio = Random.value > 0.5f;
            int random_audio = Random.Range(0, strikeAudios.Length);
            if(play_audio) rain_audioSource.PlayOneShot(strikeAudios[random_audio]);
            thunder_light.intensity = thunder_Intensity;
            striked = true;
        }
        
        thunderLight.SetActive(true);
        yield return new WaitForSeconds(Random.Range(thunderStay_Time.x, thunderStay_Time.y));
        thunderLight.SetActive(false);
         yield return new WaitForSeconds(Random.Range(thunderStay_Time.x, thunderStay_Time.y));
        thunderLight.SetActive(true);
        current_Time = Random.Range(thunderMaxTimes.x, thunderMaxTimes.y);
        yield return null;
    }
}
