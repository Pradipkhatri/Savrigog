using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour
{
    
    [SerializeField] GameObject destucted_object;
    [SerializeField] float force = 100;
    [SerializeField] float damage_to_player = 20;
    [SerializeField] AudioClip destroyAudio;
    [SerializeField] AudioSource audio_source;


    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player"){
            GameObject go = Instantiate(destucted_object, transform.position, transform.rotation);
        
            for(int i = 0; i < go.transform.childCount - 1; i++){
                go.transform.GetChild(i).GetComponent<Rigidbody>().AddForce(other.gameObject.transform.forward * force);
            }

            if(audio_source != null) {
                AudioSource asource = Instantiate(audio_source, transform.position, transform.rotation);
                asource.PlayOneShot(destroyAudio);
            }
            //if(destroyAudio != null) AudioCaller.PlayOtherSound(destroyAudio);
            //PlayerManager.Instance.advGroundChecker.slide_speed = PlayerManager.Instance.advGroundChecker.slide_speed / 2;
            PlayerManager.Instance.anim.SetTrigger("Hurt");
            PlayerManager.Instance.anim.SetFloat("HurtType", 1);
            PlayerManager.Instance.anim.SetFloat("HurtRate", 1);
            PlayerManager.Instance.FallDamage(damage_to_player, false);
            Destroy(this.gameObject);
        }
    }
}
