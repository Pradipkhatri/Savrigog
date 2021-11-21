using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegacyAnimationTrigger : MonoBehaviour
{
    [SerializeField] private bool destroy_after_done;
    [SerializeField] private Animation anim;
    [SerializeField] private string anim_name;

    [SerializeField] private float anim_speed = 1;
    [Space(10)]
    [Header("Intensity, Time")]
    [SerializeField] private bool timerIsLength;
    [SerializeField] private Vector2 camera_shake_rate = new Vector2(1, 1);

    [SerializeField] private AudioClip audio_clip;
    [SerializeField] private AudioSource audio_source;

    [SerializeField] private Transform audio_spwan_Location;

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player"){
            PlayerEntered();
        }
    }

    void PlayerEntered(){
        anim.Play(anim_name);
        anim[anim_name].speed = anim_speed;
        
        float t = 0;

        if(timerIsLength)  t = anim[anim_name].length / anim_speed; else t = camera_shake_rate.y / anim_speed;

        GameManager.gameManager.ShakeCamera(camera_shake_rate.x, t ,transform.position);

        if(audio_source != null) {
            if(!audio_spwan_Location)  audio_spwan_Location = this.transform;
            AudioSource asource = Instantiate(audio_source, audio_spwan_Location.position, transform.rotation);
            asource.PlayOneShot(audio_clip);
        }

        if(destroy_after_done){
            Destroy(this.gameObject);
        }
    }
}
