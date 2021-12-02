using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour, ITrigger
{
    
    [SerializeField] GameObject destucted_object;
    [SerializeField] float force = 100;
    [SerializeField] float damage_to_player = 20;
    [SerializeField] AudioClip destroyAudio;
    [SerializeField] AudioSource audio_source;

    PlayerManager playerManager;

    public void TriggerEnter(){
            playerManager = PlayerManager.Instance;
            GameObject go = Instantiate(destucted_object, transform.position, transform.rotation);
        
            for(int i = 0; i < go.transform.childCount - 1; i++){

                go.transform.GetChild(i).GetComponent<Rigidbody>().AddForce(playerManager.player.transform.forward * force);
            }

            if(audio_source != null) {
                AudioSource asource = Instantiate(audio_source, transform.position, transform.rotation);
                asource.PlayOneShot(destroyAudio);
            }
            //if(destroyAudio != null) AudioCaller.PlayOtherSound(destroyAudio);
            //playerManager.advGroundChecker.slide_speed = playerManager.advGroundChecker.slide_speed / 2;
            playerManager.anim.SetTrigger("Hurt");
            playerManager.anim.SetFloat("HurtType", 1);
            playerManager.anim.SetFloat("HurtRate", 1);
            playerManager.FallDamage(damage_to_player, false);
            GameManager.gameManager.impulseSource.GenerateImpulseAt(transform.position, Vector3.one  * 5);
            Destroy(this.gameObject);
    }
}
