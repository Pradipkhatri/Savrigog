using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyHealthManager : MonoBehaviour, IDamageable {

	bool scorePass = true;

	public bool no_hurtPlay = false;
	[SerializeField] [Range(0, 1)] float play_hurt = 1;
	[SerializeField] GameObject playerAvoider;
	[SerializeField] bool bowEnemy = false;

	

	[System.Serializable]
	public struct BlockProperties
	{
		public bool isBlocking;
		public int shieldLevel;
		public int max_shieldLevel;
		[Header("0 - 1")]
		public float blockPriority;
	}

	public Animator anim;
	public UnityEngine.AI.NavMeshAgent navMesh;
	EnemyAIUpdate enemyAI;
	EnemyBowShootAI enemyBowShootAI;

	[Header("Health")]
	[SerializeField] float MaxHealth;
	public float currentHealth;
	[SerializeField] float maxDefenceLevel = 50;
	[SerializeField] float defenceLevel = 50;
	[SerializeField] [Range(0, 1)] float defenceResistance = 0.2f;
	[SerializeField] GameObject enemy_ui_canvas;
	[SerializeField] Image healthFill, defenceFill;
	[SerializeField] Image healthFollow;
	[SerializeField] GameObject critical_condition_indicator;

	[Header("Blocking")]
	[SerializeField] bool canBlock = false;
	[SerializeField] private string shield_type = "Wood";

	public BlockProperties blockProperties;	
	[HideInInspector]public bool isAlive = true;
	
	[Header("Extras")]
	public bool isHurt;
	public ParticleSystem bloodParticle;
	[SerializeField] ParticleSystem blockContactParticle;
	public float knockBack = 5;
	[HideInInspector] public Quaternion LookDirection;
	
	
	public AudioSource enemy_audioSource;
	[SerializeField] private string armor_type = "Metal";
	private Transform target;
	private float Distance;

	float recoverTime = 0f;
	[SerializeField] float max_recoverTime = 0.1f;

	[SerializeField] AudioClip hurt_audio;

	void Start () {
		PlayerManager.OnSmashed += Smashed;
		navMesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
		anim = GetComponent<Animator>();
		enemyBowShootAI = GetComponent<EnemyBowShootAI>();
		
		blockProperties.shieldLevel = blockProperties.max_shieldLevel;
		enemy_audioSource = GetComponent<AudioSource>();
		currentHealth = MaxHealth;
		if(bloodParticle) bloodParticle.Play(false);
		if(!bowEnemy) enemyAI = GetComponent<EnemyAIUpdate> ();
		target = PlayerManager.Instance.player.transform;
	}
	
	
	public void ArtificialUpdate(){

		if(enemy_ui_canvas != null) enemy_ui_canvas.transform.LookAt(PlayerManager.Instance.mainCamera.transform);

		if(currentHealth <= 0){
			StopAllCoroutines();
			Death();
			isAlive = false;
			return;
		}else{
			isAlive = true;
			if(critical_condition_indicator != null){
				if(currentHealth < (MaxHealth / 4)) critical_condition_indicator.SetActive(true); else critical_condition_indicator.SetActive(false);
				critical_condition_indicator.GetComponent<Image>().color = new Color(1, 1, 1, Mathf.PingPong(Time.time * 2,1));
			} 	
		}
		
		Distance = Vector3.Distance (transform.position, target.position);
		if(recoverTime > 0) recoverTime -= Time.deltaTime;
		
		if(enemyAI != null && canBlock){
			if (enemyAI.attackTime < 0 || Distance > enemyAI.maintain_Dis.z || enemyAI.isDisturbed) { 
				blockProperties.isBlocking = false;
				blockProperties.shieldLevel = blockProperties.max_shieldLevel;
				float layerWeight = anim.GetLayerWeight(1);
				anim.SetLayerWeight (1, Mathf.Lerp (layerWeight, 0, Time.deltaTime * 4));
			}
		}
		
		if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Hurt")) isHurt = true;else isHurt = false;
		
	}
	
	public void TakeDamage(int DamageType, int Damage, bool SwordContact)
	{
		if(recoverTime > 0) return;
		recoverTime = max_recoverTime;
		
		navMesh.velocity = -transform.forward * knockBack;
		PlayArmor("Action");

		if(enemyAI != null){ 
			enemyAI.attackTime = Random.Range (0, enemyAI.maxAttackTime);
			if(enemyAI.isDisturbed){
				NotBlockingState(Damage, DamageType, SwordContact);
				return;
			}
		}
		
		if(!canBlock){
			NotBlockingState(Damage, DamageType, SwordContact);
			return;
		}
			
		if (canBlock && !enemyAI.attack_run && DamageType < 6) {
				
			if (!blockProperties.isBlocking){
				NotBlockingState (Damage, DamageType, SwordContact);
				blockProperties.isBlocking = Random.value < blockProperties.blockPriority;
			}else{
				if (blockProperties.shieldLevel > Damage){ // Has Block Counts
					BlockingState (Damage, SwordContact);
				}else { // Block Break
					//This is the blockbreak coading.
					isHurt = true;
					blockProperties.shieldLevel = blockProperties.max_shieldLevel;
					if(SwordContact) { 
						if(armor_type != "") AudioCaller.MultiGroupAudioPlay(null, enemy_audioSource, "Armor", armor_type);
						AudioCaller.MultiGroupAudioPlay(null, enemy_audioSource, "BlockBreak", shield_type);
					}	
					FilterDamage(Damage, 8, DamageType, SwordContact);
					blockProperties.isBlocking = false;
				}
			}
		}
		else
		{
			//If Does not meet condition
			NotBlockingState(Damage, DamageType, SwordContact);
		}
	}


	private void PlayHurt(int DamageType){
		bool play_anim = false;
		if(DamageType <= 4) play_anim = Random.value < play_hurt; else play_anim = true;
		if(play_anim){
			anim.SetFloat("HitType", DamageType);
			//if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Hurt")) 
			anim.SetTrigger("Hurt");
		}
	}
	private void NotBlockingState(int Damage, int DamageType, bool SwordContact){
		isHurt = true;
		if(SwordContact) {
			if(armor_type != "") AudioCaller.MultiGroupAudioPlay(null, enemy_audioSource, armor_type, "Action");
			AudioCaller.MultiGroupAudioPlay(PlayerManager.Instance.swordAudios, null, PlayerManager.Instance.current_weapon_type, PlayerActions.wallType);
		}
		FilterDamage(Damage, 0, DamageType, SwordContact);
		
	}
	
	public void FilterDamage(int Damage, int Additional, int DamageType, bool SwordContact){

		if(defenceLevel > 0){
			float new_damage = Damage * defenceResistance;
			currentHealth -= Damage - new_damage; 
			defenceLevel -= new_damage + Additional;
		}else{
			currentHealth -= Damage + Additional;
		}
		
		if(!no_hurtPlay) PlayHurt(DamageType);

		if(healthFill != null) healthFill.fillAmount = currentHealth / MaxHealth;
		if(defenceFill != null) defenceFill.fillAmount = defenceLevel / maxDefenceLevel;
		if(bloodParticle && SwordContact) bloodParticle.Play (true);
		if(hurt_audio != null) AudioCaller.PlaySound(enemy_audioSource, hurt_audio);
		FilterBar();

	}

	
	private void BlockingState(int DamageRate, bool SwordContact)
	{
		
		blockProperties.shieldLevel -= DamageRate;
		anim.SetLayerWeight (1, 1);
		//navMesh.velocity = -transform.forward * knockBack/2;
		//anim.SetBool("Blocking", true);
		anim.SetTrigger("Impact");
		transform.rotation = Quaternion.Slerp(transform.rotation, LookDirection, 5 * Time.deltaTime);
		
		if(SwordContact) AudioCaller.MultiGroupAudioPlay(null, enemy_audioSource, "Block", shield_type);
		if(blockContactParticle) blockContactParticle.Play();
	}
	
	void Smashed(int damageRate){
		if(Distance < (PlayerManager.Instance.damageType - 2)){
			navMesh.velocity = -transform.forward * 10;
			PlayArmor("Action");
			TakeDamage(PlayerManager.Instance.damageType, damageRate, false);
		}
	}

	private void Death(){
		

		navMesh.enabled = false;
		if(enemy_ui_canvas != null) Destroy(enemy_ui_canvas);
		if(playerAvoider != null) playerAvoider.SetActive(false);
		PlayerManager.OnSmashed -= Smashed;
		this.GetComponent<CapsuleCollider>().enabled = false;
		if(scorePass == true){
				
				EnemySpwan enemySpwan = gameObject.GetComponentInParent<EnemySpwan>();
				if(enemySpwan != null) enemySpwan.CheckRemainingEnemies();

				bool spwanObject = Random.value > 0.5f;
				
				if(spwanObject) Instantiate(GameManager.gameManager.enemy_itemSpwans[Random.Range(0, GameManager.gameManager.enemy_itemSpwans.Count)] ,transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
				PlayerManager.Instance.scoreStore += 1.3f;
				transform.SetParent(null);
				scorePass = false;
		}
		anim.SetLayerWeight (1, 0);
		anim.SetBool("IsDead", true);
		Destroy(this.gameObject, 5);
	}

	private void BodyDrop(){
		if(armor_type != null) AudioCaller.MultiGroupAudioPlay(null, enemy_audioSource, "ArmorHit", armor_type);
		PlayArmor("Action");
		AudioCaller.SingleGroupAudioPlay(null, enemy_audioSource, "BodyDrop");
	}

	public void PlayArmor(string code){
		if(armor_type != "") AudioCaller.MultiGroupAudioPlay(null, enemy_audioSource, armor_type, code);
	}

	private IEnumerator BarFollow(){
		float delayTimer = 0;
		float lerpTime = 0;
		while(healthFollow.fillAmount > healthFill.fillAmount){
			delayTimer += Time.deltaTime;
			if(delayTimer > 1){
				lerpTime += Time.deltaTime;
				float percentage = lerpTime / 1.5f;
				healthFollow.fillAmount = Mathf.Lerp(healthFollow.fillAmount, healthFill.fillAmount, percentage);
				yield return null;
			}
			yield return null;
		}
	}

	private void FilterBar(){
		if(healthFollow.fillAmount > healthFill.fillAmount){
			StopAllCoroutines();
			StartCoroutine(BarFollow());
		}else if(healthFollow.fillAmount <= healthFill.fillAmount){
			healthFollow.fillAmount = healthFill.fillAmount;
		}

	}
}
