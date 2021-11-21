using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGiant : MonoBehaviour
{
    private EnemyAIUpdate enemyAI;
    [SerializeField] float groundShakeVelocity = 0.1f;
    [SerializeField] AudioClip[] stepAudios;
    Animator anim;

    [SerializeField] AudioClip sword_clash_audio;

    int prev_random = 0;
    private void Start()
    {
        enemyAI = GetComponent<EnemyAIUpdate>();
        anim = GetComponent<Animator>();
    }
	
    private void GiantFoot(){
      //  if(anim.GetFloat("Movement") > 0.8f || anim.GetFloat("Movement") < -0.8f){
            GameManager.gameManager.impulseSource.GenerateImpulseAt(transform.position, new Vector3(groundShakeVelocity, groundShakeVelocity, groundShakeVelocity));
            enemyAI.enemyAudioSource.PlayOneShot(stepAudios[RandomNumber(stepAudios.Length)]);
            GetComponent<EnemyHealthManager>().PlayArmor("FootSteps");
       // }
	}

	private int RandomNumber(int length){
        int random = Random.Range(0, length);
        if(random ==  prev_random){
            int new_random;
            if(random >= length - 1){
                new_random = 0;
            }else{
                new_random = length - 1;
            }

            prev_random = new_random;
            return new_random;
        }else{
            prev_random = random;
            return random;
        }
    }
}
