using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeadShot : MonoBehaviour
{
    // Start is called before the first frame update

    EnemyHealthManager enemyHealth;
    public int helmetLevel = 1;

    AudioSource audioSource;

    [SerializeField] GameObject helmet;

    [SerializeField] AudioClip headShot_Audio;
    [SerializeField] AudioClip helmetHit_Audio;

    GameObject head_arrow_clone;

    void Start(){
        enemyHealth = gameObject.GetComponentInParent<EnemyHealthManager>();
        audioSource = gameObject.GetComponentInParent<AudioSource>();

        head_arrow_clone = transform.GetChild(0).gameObject;
        if(head_arrow_clone != null) head_arrow_clone.SetActive(false);
    }

    public void TakenDamage(int damageRate){
        if(helmetLevel > 0){
            helmetLevel -= 1;
            if(helmetLevel == 1 && helmet != null) Destroy(helmet);
            if(helmetHit_Audio != null) audioSource.PlayOneShot(helmetHit_Audio);

            enemyHealth.FilterDamage (damageRate + 10, 0, 2, false);
        }else{
            if(headShot_Audio != null) audioSource.PlayOneShot(headShot_Audio);
            enemyHealth.FilterDamage (damageRate * 2 + 200, 0, 2, false);
            if(enemyHealth.currentHealth < 10 && head_arrow_clone != null) head_arrow_clone.SetActive(true);
        }
    }
}
