using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
    
    [SerializeField] int arrowSpeed = 20;
    [SerializeField] int damageRage = 20;
    [SerializeField] float destroyTime = 5;
    [SerializeField] float rayLength = 1f;
    [HideInInspector] public bool collided = false;
    [SerializeField] ParticleSystem brustParticle;
    public GameObject arrowHole;
    AudioSource audioSource;
    [SerializeField] AudioClip hitAudio;

    void Start(){
         audioSource = GetComponent<AudioSource>();
    }
    void FixedUpdate()
    {
        if(!collided){
            transform.Translate(Vector3.forward * arrowSpeed * Time.deltaTime);
            Rayshoot();
        }
            
        Destroy(this.gameObject, destroyTime);
    }

    void Rayshoot(){
        RaycastHit hit;
        
        Ray ray = new Ray(transform.position, transform.forward);

        if(Physics.Raycast(ray, out hit, rayLength)){
            if(!collided) {
                if(hit.transform.CompareTag("Player")){
                    PlayerManager.Instance.TakeDamage(damageRage, 1, this.transform);
                //}else if(hit.transform.CompareTag("Enemy")){
                   // hit.transform.GetComponent<EnemyHealthManager>().TakeDamage(1, 1, false);
                }else{
                    audioSource.PlayOneShot(hitAudio);
                    Instantiate(brustParticle, hit.point - (transform.forward * 0.3f), Quaternion.LookRotation(-transform.forward));
                    //transform.parent = hit.transform;
                    //transform.position = transform.position + (transform.forward * 0.35f);
                    //GameObject bulletHole = Instantiate(arrowHole, hit.point - (transform.forward * 0.01f), Quaternion.LookRotation(-hit.normal));
                }
               
            }
            collided = true;
            Destroy(this.gameObject);
        }
        
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * rayLength);
    }
}
