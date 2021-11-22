using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyAIUpdate : MonoBehaviour {

	[Tooltip("Enable if you want to update rotation while attaking")]
	[SerializeField] bool attack_flexible = false;
	[SerializeField] bool enable_root_motion = true;
	[Tooltip("Settings are for maintaining navMesh stoppingDistance & AttackingRange")]
	[SerializeField] float walkSetting = 1.5f;
	[SerializeField] float runSetting = 2.5f;

	private EnemyHealthManager enemyManager;
	Animator anim;
	NavMeshAgent navMesh;
	[HideInInspector] public AudioSource enemyAudioSource;
	NavMeshPath path;
	public bool isDisturbed = false;

	[Header("ChasingProperties")]
	Transform target;
	[SerializeField] int upCheckRange = 1;
	[SerializeField] bool isRunning = false;
	[Tooltip("Strafe, Walk, Run")]
	[SerializeField] Vector3 movement_Speed = new Vector3(2, 3, 6);
	[Tooltip("MaintainDis, MinDis, MaxDis")]
	public Vector3 maintain_Dis = new Vector3(3, 3, 7);
	[Tooltip("CurrentDis, ChaseRange, AttackRange")]
	[SerializeField] Vector3 range_calc = new Vector3(0f, 20f, 0.8f);
	Quaternion lookDirection;


	[Header("AttackFormalities")]
	public bool attack_run;
	[SerializeField] Vector2Int max_min_attackType = new Vector2Int(0, 4);
	private bool striking;
	[SerializeField] float lookatSpeed = 8;
	float defLookatSpeed = 8;
	public float attackTime = 5;
	public float maxAttackTime = 10;
	[SerializeField] Vector3 damageRange = new Vector3(0.8f, 0.7f, 1);
	[SerializeField] float hitDamage = 10;


	[Header("StrafeProperties")]
	[SerializeField] bool can_Strafe = true;
	//Going to have 6 random numbers, where 0 is strafe left! 3 strafe forward! And 6 is strafe right.
	bool isStrafing = false;
	[SerializeField] float random_generator;
	bool generated = false;
	[Tooltip("StrafeTimer, min_strafeTime, max_strafeTime")]
	[SerializeField] Vector3 strafe_time = new Vector3(0f, 1f, 3f);
	[SerializeField] float next_strafe_time = 1;

	[Header("AudioClips")]
	[SerializeField] string weapon_type = "Sword";
	[SerializeField] string weapon_weight = "Medium";
	[SerializeField] GameObject sword_trail;
	[SerializeField] LayerMask playerLayer;
	

	void Start () {
		defLookatSpeed = lookatSpeed;
		target = PlayerManager.Instance.player.transform;
		enemyManager = GetComponent<EnemyHealthManager>();
		anim = enemyManager.anim;
		navMesh = enemyManager.navMesh;
		enemyAudioSource = GetComponent<AudioSource>();
		path = new NavMeshPath();
		if(sword_trail != null) sword_trail.SetActive (false);
	}


	void Update () {
		if(!enemyManager || !navMesh.enabled || enemyManager.justSpwaned) return;

		enemyManager.ArtificialUpdate();
		if(!enemyManager.isAlive){
			return;
		}
		

		if(!enemyManager.isHurt) LookAtPlayer(); else{
			maintain_Dis.x = range_calc.x;
			if(sword_trail) sword_trail.SetActive(false);
		} 

		if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack")){
			striking = true;
			enemyManager.no_hurtPlay = true;
		}else{
			striking = false;
			enemyManager.no_hurtPlay = false;
		}

		if (striking || enemyManager.isHurt){
			Guard();
			if(enable_root_motion) anim.applyRootMotion = true;
			isDisturbed = true;
		}else{
			if(enable_root_motion) anim.applyRootMotion = false;
			isDisturbed = false;
		}

		if(isRunning) {
			navMesh.speed = movement_Speed.z;
			navMesh.stoppingDistance = runSetting;
			range_calc.z = runSetting;
		}else if(isStrafing){
			navMesh.speed = movement_Speed.x;
		}else{
			navMesh.speed = movement_Speed.y;
			navMesh.stoppingDistance = walkSetting;
			range_calc.z = walkSetting;
		}

		if(!isStrafing) anim.SetFloat("Strafe", 0, 0.1f, Time.deltaTime);

		if(attackTime <= 0 || striking) attack_run = true; else attack_run = false;

		range_calc.x = Vector3.Distance(transform.position, target.position);

		if(PlayerDetector() && !isDisturbed && !PlayerManager.Instance.isDead){
			if(!isDisturbed) attackTime -= Time.deltaTime;
			if(attack_run){
				Attack();
				ResetStrafe();
				//if(enable_root_motion) anim.applyRootMotion = true;
				navMesh.autoBraking = true;
			}else{
				Chase();
				navMesh.autoBraking = false;
			}
		}else Guard();
	}

	void LookAtPlayer(){
		if (striking || enemyManager.isHurt)
			if(attack_flexible) lookatSpeed = defLookatSpeed / 5; else lookatSpeed = 0;
		else
			lookatSpeed = defLookatSpeed;

		Vector3 Dir = target.position - transform.position;
		Dir.y = 0;
		lookDirection = Quaternion.LookRotation(Dir);
		transform.rotation = Quaternion.Lerp(transform.rotation, lookDirection, lookatSpeed * Time.deltaTime);
	}

	void Chase(){
		if(isRunning) StopRunning();
		
		if(can_Strafe){
			if (random_generator <= 0 && range_calc.x > navMesh.stoppingDistance) {
				if(!generated) {
					Strafe(Random.Range (0, 6));
					generated = true;
				}else{
					if(!isStrafing) ResetStrafe();
				}
			}
		}

		Follow();
	}

	private void Follow(){
		NavMeshHit hit;

		//Enemy Following the Player
		if(range_calc.x > maintain_Dis.x + navMesh.stoppingDistance){
			navMesh.destination = target.position + (transform.forward * maintain_Dis.x);
			anim.SetFloat("Movement", 1.1f, 0.1f, Time.deltaTime);
		}

		//Enemy Idle Position in the middle of battle!	
		if(range_calc.x >= maintain_Dis.x - navMesh.stoppingDistance && range_calc.x <= maintain_Dis.x + navMesh.stoppingDistance){
			if(!isStrafing) BattleSituationIdle();
		}

		//Retreat and Enemy Maintaining Distance Form the Player
		if(range_calc.x < maintain_Dis.x - navMesh.stoppingDistance){
			
			Vector3 backPos = target.position - (transform.forward * maintain_Dis.x);
			bool isvalid = NavMesh.SamplePosition(backPos, out hit, .1f, NavMesh.AllAreas);
			if(isvalid){
				navMesh.updateRotation = false;
				navMesh.destination = target.position - (transform.forward * maintain_Dis.x);
				anim.SetFloat("Movement", -1, 0.1f, Time.deltaTime);
			}else{
				if(!isStrafing) BattleSituationIdle();
			}
		}else navMesh.updateRotation = true;
	}

	#region strafeRegion
	void Strafe(int strafe_type){
		switch(strafe_type){
		case 1:
			StartCoroutine(DoStrafe(-1));
			break;
		case 2:
			StartCoroutine(DoStrafe(1));
			break;
		case 3:
			ResetStrafe();
			break;
		case 4:
			StartCoroutine(DoStrafe(-1));
			break;
		case 5:
			StartCoroutine(DoStrafe(1));
			break;
		default:
			ResetStrafe();
			break;
		}
	}

	private IEnumerator DoStrafe(int i){
		
		int new_i = i;
		NavMeshHit hit;
		isStrafing = true;

		while(strafe_time.x > 0 && !attack_run &&  enemyManager.isAlive == true){
				strafe_time.x -= Time.deltaTime;
				Vector3 offsetPlayer = transform.position - (target.transform.position + transform.forward * 3);
				Vector3 dir = Vector3.Cross(offsetPlayer, Vector3.up);
				Vector3 newPos = transform.position + (dir * new_i);
				bool is_valid = NavMesh.SamplePosition(newPos, out hit, 0.1f, NavMesh.AllAreas);
				if(is_valid){
					navMesh.destination = newPos;
					anim.SetFloat("Strafe", (-1 * new_i), 0.1f, Time.deltaTime);
				}else{
					navMesh.destination = transform.position;
					anim.SetFloat("Strafe", 0);
				}
				yield return null;
		}
		yield return null;
		ResetStrafe();
	}

	private void ResetStrafe(){
		StopCoroutine("DoStrafe");
		generated = false;
		maintain_Dis.x = Random.Range (maintain_Dis.y, maintain_Dis.z);
		strafe_time.x = Random.Range(strafe_time.y, strafe_time.z);
		random_generator = next_strafe_time;
		isStrafing = false;
	}
	#endregion


	void BattleSituationIdle(){
		if(random_generator > 0) random_generator -= Time.deltaTime; 
		navMesh.destination = transform.position;
		anim.SetFloat("Movement", 0, 0.1f, Time.deltaTime);
		navMesh.speed = 0;
	}
		

	void Guard(){
		navMesh.destination = transform.position;
		navMesh.speed = 0;
		anim.SetFloat("Movement", 0, 0.1f, Time.deltaTime);
	}

	private void Attack(){
		if(PlayerManager.Instance.isDead) return;
		ResetStrafe();
		//maintain_Dis.x = range_calc.z;
		if(attackTime <= -10 && !striking){
			attackTime = Random.Range (0, maxAttackTime);
			return;
		}

		//If Distance is lower than AttackRange!
		if(!striking){
		if (range_calc.x < range_calc.z) {
			StopRunning();
			int attackType = Random.Range (max_min_attackType.x, max_min_attackType.y);
			anim.SetFloat ("AttackForce", attackType);
			anim.SetTrigger ("Attack");
			attackTime = Random.Range (0, maxAttackTime);
		} else if (range_calc.x > range_calc.z) {
				isRunning = true;
				navMesh.updateRotation = true;
				navMesh.destination = target.position;
				anim.SetFloat ("Movement", 2f, 0.1f, Time.deltaTime);
		}
		}
	}

	private void StopRunning(){
		isRunning = false;
		navMesh.updateRotation = false;
		anim.SetFloat ("Movement", 0);
	}

	private void DamagePoint(int AttackType){
		
		if(!enemyManager.isHurt){
			Vector3 pos = transform.position + ((Vector3.up * damageRange.y) +  (transform.forward * damageRange.x));
			if(Physics.CheckSphere(pos, damageRange.z, playerLayer)){
				if(!PlayerManager.Instance.isBlocking && !PlayerActions.isDouging) 
					AudioCaller.MultiGroupAudioPlay(PlayerManager.Instance.swordAudios, enemyAudioSource, weapon_type, "Flesh");
				PlayerManager.Instance.TakeDamage(hitDamage, AttackType, this.transform);
			}
		}
	}

	private bool PlayerDetector (){

		//if(range_calc.x > range_calc.y) return false;

		navMesh.CalculatePath(target.position, path);
		RaycastHit hit;

		if(path.corners.Length != 0){
			Debug.DrawRay(path.corners[path.corners.Length - 1], Vector3.up * upCheckRange, Color.green);
			Ray upCheck = new Ray(path.corners[path.corners.Length - 1], Vector3.up * upCheckRange);
			
			
			if(Physics.Raycast(upCheck, out hit, upCheckRange)){
				if(hit.transform.CompareTag("Player")) return true; else return false;
			}else return false;
		} else return false;
		
	}

	void OnDrawGizmos(){
		//Gizmos.DrawWireSphere(transform.position, range_calc.y);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position + ((Vector3.up * damageRange.y) +  (transform.forward * damageRange.x)), damageRange.z);
	}

	public void FootStep(){
		if(anim.GetFloat("Movement") > 0.9 || anim.GetFloat("Movement") < -0.8 || anim.GetFloat("Strafe") > 0.8 || anim.GetFloat("Strafe") < -0.8){
			enemyManager.PlayArmor("FootSteps");
			AudioCaller.MultiGroupAudioPlay(null, enemyAudioSource, "FootSteps", PlayerActions.groundType);
		} 
	}

	private void TrailStart(){
		enemyManager.PlayArmor("Action");
		if(sword_trail != null) sword_trail.SetActive(true);
		AudioCaller.MultiGroupAudioPlay(PlayerManager.Instance.swordAudios, enemyAudioSource, "Swoosh", weapon_weight);
	}

	private void TrailEnd(){
		if(sword_trail != null) sword_trail.SetActive(false);
		maintain_Dis.x = Random.Range (maintain_Dis.y, maintain_Dis.z);
	}	

	private void SwordGroundContact(){
		TrailEnd();
		AudioCaller.MultiGroupAudioPlay(PlayerManager.Instance.swordAudios, enemyAudioSource, "GroundClash", weapon_weight);
	}

}
