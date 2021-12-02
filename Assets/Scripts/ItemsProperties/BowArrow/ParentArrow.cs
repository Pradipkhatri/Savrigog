using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentArrow : MonoBehaviour
{
    public ParentArrowProperties parent_arrow_properties;

    bool collided = false;

    [SerializeField] float destroyTime = 5;

    RaycastHit hit;

    [SerializeField] AudioSource audioSouce;

    [SerializeField] float calculated_damage;
    [SerializeField] float calculated_speed;

    public void Initialize(){
        BowMech bm = FindObjectOfType<PlayerShooting>().current_bow_properties;
        calculated_damage = parent_arrow_properties.damageRate * bm.power_multiplyer * bm.current_charge_state;
        calculated_speed = bm.current_charge_state + parent_arrow_properties.arrowSpeed;
    }

    private void OnEnable(){
        Invoke("DestroyNow", destroyTime);
        Application.onBeforeRender += ArrowUpdate;
    }

    private void OnDisable(){
        Application.onBeforeRender -= ArrowUpdate;
    }

    private void ArrowUpdate()
    {
        ShootedArrow();
    }

    void DestroyNow(){
        Destroy(parent_arrow_properties.child_arrow_object.gameObject);
        Destroy(this.gameObject);
    }

    private void ShootedArrow(){
        if(parent_arrow_properties.child_arrow_object == null) return;
        if(!Collided()){
            ChildArrowFollow();
            transform.Translate(Vector3.forward * calculated_speed * Time.deltaTime);
            if(parent_arrow_properties.fallTimer > 0) 
                parent_arrow_properties.fallTimer -= Time.deltaTime;
            else
                transform.rotation = transform.rotation * Quaternion.Euler(parent_arrow_properties.gravity,0,0);
        }else ArrowCollided();
    }

    private void ChildArrowFollow(){
        float follow_speed = parent_arrow_properties.child_arrow.followSpeed;

        if(parent_arrow_properties.child_arrow.waitTime > 0) {
            parent_arrow_properties.child_arrow.waitTime -= Time.deltaTime;
            parent_arrow_properties.child_arrow_object.Translate(Vector3.forward * calculated_speed * Time.deltaTime);
            //parent_arrow_properties.child_arrow_object.position += parent_arrow_properties.child_arrow_object.forward * parent_arrow_properties.arrowSpeed * Time.deltaTime;
        }else{
            parent_arrow_properties.child_arrow_object.position = Vector3.Lerp(parent_arrow_properties.child_arrow_object.position, transform.position, follow_speed * Time.deltaTime);
            parent_arrow_properties.child_arrow_object.rotation = Quaternion.Lerp(parent_arrow_properties.child_arrow_object.rotation, transform.rotation, (follow_speed/2) * Time.deltaTime);
        }
    }

    private bool Collided(){
        Ray ray = new Ray(transform.position, transform.forward);

        if(Physics.SphereCast(transform.position, parent_arrow_properties.sphereCast_Size, transform.forward, out hit, parent_arrow_properties.rayLength, parent_arrow_properties.enemyLayers)){
            if(hit.transform.CompareTag("Enemy") && !collided){
                FilterHitBody(hit);
                return true;
            }   
            else if(hit.transform.CompareTag("Head") && !collided){
                FilterHitHead(hit);
                return true;
            }
            audioSouce.PlayOneShot(parent_arrow_properties.hitAudio);
        }

        if(Physics.Raycast(ray, out hit, parent_arrow_properties.rayLength, parent_arrow_properties.collisionLayers)){
            if(!collided) {
                //GameObject bulletHole = Instantiate(parent_arrow_properties.arrowHole, hit.point - (transform.forward * 0.01f), Quaternion.LookRotation(-hit.normal));
                //bulletHole.transform.SetParent(hit.transform);
                //ParticleSystem brustPar = Instantiate(parent_arrow_properties.brustParticle, hit.point, Quaternion.LookRotation(-transform.forward));
                
                GameObject brustPar = SpwanManager.Instance.SpwanFromPool(parent_arrow_properties.brust_particle_name, hit.point, Quaternion.LookRotation(-transform.forward), true);
			    brustPar.GetComponent<ParticleSystem>().Play();

                transform.SetParent(hit.transform);
            }
            transform.position = hit.point;
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            return true;
        }

        return false;
        
    }

    private void FilterHitBody(RaycastHit hit){
        IDamageable damageable = hit.transform.GetComponent<IDamageable>();
        if(damageable == null) return;
        damageable.TakeDamage (2 , Mathf.CeilToInt(calculated_damage), false);
        //ParticleSystem brustPar = Instantiate(parent_arrow_properties.bloodParticle, hit.point, Quaternion.LookRotation(-transform.forward));
        collided = true;
    }

    private void FilterHitHead(RaycastHit hit){
        EnemyHeadShot enemyHeadShot = hit.transform.GetComponent<EnemyHeadShot>();
        enemyHeadShot.TakenDamage(Mathf.CeilToInt(calculated_damage));
        //ParticleSystem brustPar = Instantiate(parent_arrow_properties.bloodParticle, hit.point, Quaternion.LookRotation(-transform.forward));
        collided = true;
    }

    void ArrowCollided()
    {
        if(!collided){
            audioSouce.PlayOneShot(parent_arrow_properties.hitAudio);
            parent_arrow_properties.child_arrow_object.position = hit.point - (transform.forward * 0.1f);
            parent_arrow_properties.child_arrow_object.GetComponent<ChildArrow>().Collided();
            collided = true;
            return;
        }else return;
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.forward * parent_arrow_properties.rayLength, parent_arrow_properties.sphereCast_Size);
        Gizmos.DrawRay(transform.position, transform.forward * parent_arrow_properties.rayLength);
    }
}
