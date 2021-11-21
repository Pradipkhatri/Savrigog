using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyBowShootAI : MonoBehaviour {

    private EnemyHealthManager enemyHealthManager;
    private Transform _player;
    [SerializeField] float aimHight = 1.7f;   
    private Animator anim;
    //[SerializeField] Animator bowAnimator;

    NavMeshAgent navMesh;

    [Tooltip("Current Distance, closeupRange, detectRange")]
	[SerializeField] Vector3 detectRange = new Vector3 (0, 7, 40);

    [Tooltip("RecoverTime, Max_RecoverTime, Aim_Duration : Aim_dutation should be in minus because it is counted after the recovertime lowers than zero")]
    [SerializeField] Vector3 attackTime = new Vector3(0, 10, -5);
    [SerializeField] float releaseTime = -5;

    [SerializeField] float rotationSpeed = 2;
    [SerializeField] Vector3 lookOffset;

    bool aiming = false;

    [Header ("Shooting Settings")]
    [SerializeField] Transform spwanPoint;
    [SerializeField] Transform arrowPrefab;
    [SerializeField]  int bulletSpeed;

    [Header("Audio Options")]

    AudioSource audioSource; 
    [SerializeField] AudioClip bowStretch_Audio;
    [SerializeField] AudioClip bowShoot_Audio;
    [SerializeField] AudioClip bodyDrop;

    [SerializeField] GameObject fakeArrow;
    private void Start(){
        attackTime.x = attackTime.y;
        fakeArrow.SetActive(false);
        enemyHealthManager = GetComponent<EnemyHealthManager>();
        navMesh = enemyHealthManager.navMesh;
        audioSource = GetComponent<AudioSource>();
        _player = PlayerManager.Instance.player.transform;
        anim = enemyHealthManager.anim;
    }  

    private void Update()
    {
        if(!enemyHealthManager || !_player || !anim) return;
        enemyHealthManager.ArtificialUpdate();
        if(!enemyHealthManager.isAlive) return;

        detectRange.x = Vector3.Distance(transform.position, _player.position);
        
        if(anim.GetCurrentAnimatorStateInfo(1).IsTag("Aiming") || anim.GetNextAnimatorStateInfo(1).IsTag("Aiming")){
            aiming = true;
        }else{
            aiming = false;
        }

        if(enemyHealthManager.isHurt){
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0, 8 * Time.deltaTime));
            attackTime.x = Random.Range(1, attackTime.y);
            attackTime.z = Random.Range(-1, releaseTime);
        }else{
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 1, 8 * Time.deltaTime));
        }

        if(detectRange.x < detectRange.z){
            if(detectRange.x < (detectRange.y - 2 )) {
                if(!enemyHealthManager.isHurt) MaintainDistance();
            }else{
                    BattleCondition();  
            }
        }else Idle();
    }

    private void MaintainDistance(){	
        PlayerLookAt();
        anim.SetBool("Aim", false);
        navMesh.updateRotation = false;
        navMesh.SetDestination(_player.position + (-transform.forward * detectRange.y));
		anim.SetFloat("Movement", -1, 0.1f, Time.deltaTime);
        
    }

    private void PlayerLookAt(){
      if(!aiming){
            Vector3 lookDir = _player.position - transform.position;
            lookDir.y = 0;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * rotationSpeed);
      }else{
          
            spwanPoint.LookAt(_player.position + (Vector3.up * aimHight));
            transform.LookAt(_player.position); 
            Quaternion newRotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
            transform.rotation = newRotation * Quaternion.Euler(lookOffset);
            
      }
    }

    private bool PlayerDetector(){
            RaycastHit hit;

            Debug.DrawLine(spwanPoint.position, _player.position + (Vector3.up * aimHight), Color.red);

            if(Physics.Linecast(spwanPoint.position, _player.position + (Vector3.up * aimHight), out hit)){
                    if(hit.transform.CompareTag("Player")){
                        return true;
                    }else{
                        return false;
                    }
            }else {
                return false;
            }
    }
    private void Idle(){
        anim.SetLayerWeight(1, 0);
        anim.SetBool("Aim", false);
        anim.SetFloat("Movement", 0, 0.1f, Time.deltaTime);
    }
    private void BattleCondition(){
        attackTime.x -= Time.deltaTime;
        navMesh.updateRotation = true;
		navMesh.SetDestination(transform.position);
        PlayerLookAt();
        Idle();

        if(attackTime.x < 0){
            BowAim();
        }
    }

    private void BowAim(){
        anim.SetLayerWeight(1, 1);
        fakeArrow.SetActive(true);
        anim.SetBool("Aim", true);
        
        if(attackTime.x < attackTime.z){
            if(PlayerDetector()){
                BowShoot();
            }
        }

        if(attackTime.x < (attackTime.z - 5) ){
            attackTime.x = Random.Range(1, attackTime.y);
            attackTime.z = Random.Range(-1, releaseTime);
        }

    }

    private void BowShoot(){
        fakeArrow.SetActive(false);
        anim.SetTrigger("Shoot");
        //bowAnimator.Play("Bow_Shoot");
        audioSource.PlayOneShot(bowShoot_Audio);
        Instantiate(arrowPrefab, spwanPoint.position, spwanPoint.rotation);
        attackTime.x = Random.Range(1, attackTime.y);
        attackTime.z = Random.Range(-1, releaseTime);
    }

    private void BowStretch(){
        audioSource.PlayOneShot(bowStretch_Audio);
        //bowAnimator.Play("Bow_Stretch");
    }

    void BodyDrop(){
		audioSource.PlayOneShot (bodyDrop);
	}
}
