using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldProperty : MonoBehaviour
{

    public int shield_Level = 1;
    [SerializeField] string shieldDeploy_Anim;
    [SerializeField] string shieldClose_Anim;
    [SerializeField] float animSpeed;

    [SerializeField] ParticleSystem particleEffect;
    [SerializeField] AudioClip deployAudio;
    [SerializeField] AudioClip closeAudio;
    Animation anim;

    bool trigger = false;

    public string shield_Mat = "Metal";

    void Start(){
        anim = GetComponent<Animation>();
    }

    public void Block()
    {
        if(anim == null) return;

        anim[shieldDeploy_Anim].time = Mathf.Clamp(anim[shieldDeploy_Anim].time, 0, 0.5f);

        anim.Play(shieldDeploy_Anim);
        if(trigger == false){
            if(particleEffect != null) particleEffect.Play(true);
            if(deployAudio != null) AudioCaller.PlayOtherSound(deployAudio);
            trigger = true;
        }
        anim[shieldDeploy_Anim].speed = animSpeed;
        
    }

    public void UnBlock(){
        anim.Play(shieldDeploy_Anim);
        if(trigger == true){
            if(closeAudio != null) AudioCaller.PlayOtherSound(closeAudio);
                trigger = false;
            }
        anim[shieldDeploy_Anim].speed = -1 * animSpeed;
    }
}
