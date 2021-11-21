using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour {
	#region Singleton
	private static PlayerManager _instance;

	public static PlayerManager Instance{
		get{
			return _instance;
		}
	}
	#endregion

	void Awake(){
		if(_instance == null){
			_instance = this;
		}else{
			Destroy(this.gameObject);
		}
		player = this.gameObject;
		playerHealth = playerMaxHealth;
		InitializeScripts();
		
	}
	
	[SerializeField] Rect refField = new Rect(0,0,100,100);

	#region ParticleSystemProperties
	[System.Serializable]
	public struct ParticleSystems{
		public string particleName;
		public ParticleSystemManager.ParticleList particleList;
		public ParticleSystem particlePrefab;
	}
	public ParticleSystems[] particleClipArray;
	#endregion

	[SerializeField] private LayerMask enemyLayer;
	public SingleGroupLibrary playerAudios;
	public AudioLibrary mainAudios;
	public AudioLibrary swordAudios;
	public string armor_type = "Metal";

	[SerializeField] private Vector3 enemyChecker_Pos_Size = new Vector3(5, 5, 0);

	[Tooltip("Force back when player got hit.")]
	[SerializeField] int knock_back_force;

	
	public float playerMaxHealth = 100;
	public float maxStaminaLevel = 100;
	public float staminaRecoverSpeed = 40;

	[Header("UI Settings")]
	[SerializeField] Image staminaBar_Level;
	[SerializeField] private Image healthBar;

	[SerializeField] ShieldProperty current_Shield_Property;


	bool enableGUI = false;

	
	[HideInInspector] public float current_slope;
	[HideInInspector] public int RHS_weapon_Level = 1;
	[HideInInspector] public bool isBlocking;
	[HideInInspector] public bool swordStuck = false;
	[HideInInspector] public int damageType;
	[HideInInspector] public int currentDamageRate;
	[HideInInspector] public bool damageEnabled = false;
	[HideInInspector] public bool controlAbleJump = true;
	[HideInInspector] public float stamina_hold_period = 3;
	[HideInInspector] public float currentStamina = 100;
	[HideInInspector] public float scoreStore;
	[HideInInspector] public bool shootReady;
	[HideInInspector] public bool isDead = false;
	[HideInInspector] public Animator anim;
	[HideInInspector] public string current_weapon_type;
	[HideInInspector] public Transform current_enemy;
	[HideInInspector] public Vector3 hit_normal;
	[HideInInspector] public CharacterController cc;
	[HideInInspector] public Transform mainCamera;
	[HideInInspector] public GameObject player;
	[HideInInspector] public float playerHealth = 100;
	[HideInInspector] public float slide_speed;


	//Scripts
	[HideInInspector] public CharacterMovement cm;
	[HideInInspector] public PlayerAttack playerAttack;
	[HideInInspector] public LadderClimbing ladderClimbing;
	[HideInInspector] public ClimbingWall climbingWall;
	[HideInInspector] public AdvGroundChecker adv_groundChecker;


	public delegate void OnGroundSmash(int damageRate);
	public static event OnGroundSmash OnSmashed;
	
	void InitializeScripts(){
		cc = GetComponent<CharacterController>();
		cm = GetComponent<CharacterMovement>();
		ladderClimbing = GetComponent<LadderClimbing>();
		climbingWall = GetComponent<ClimbingWall>();
		playerAttack = GetComponent<PlayerAttack>();
		adv_groundChecker = GetComponent<AdvGroundChecker>();
		anim = GetComponent<Animator>();
		mainCamera = Camera.main.transform;
	}

	public void Smashed(){
		if(OnSmashed != null) OnSmashed(currentDamageRate);
	}

	void StaminaManager(){
		staminaBar_Level.fillAmount = currentStamina / maxStaminaLevel;
		currentStamina = Mathf.Clamp (currentStamina, 0, maxStaminaLevel);
		stamina_hold_period = Mathf.Clamp (stamina_hold_period, 0, maxStaminaLevel);

		if(stamina_hold_period > 0) stamina_hold_period -= Time.deltaTime;
		if (currentStamina < maxStaminaLevel && stamina_hold_period <= 0.1f && !isBlocking) {
			currentStamina += Time.deltaTime * staminaRecoverSpeed;
		}
	}

	void Update(){
		if(isDead || GameManager.gameManager.isPaused){
			cc.Move(Vector3.up * cm.velocityY);
			return;
		}
		if(cm) cm.ArtificialUpdate();
		if(playerAttack) playerAttack.ArtificialUpdate();
	}

    //[System.Obsolete]
    void FixedUpdate(){		
			if(isDead || GameManager.gameManager.isPaused) {
				cc.Move(Vector3.up * cm.velocityY);
				return;
			}
			RootMotionHandler();
			StaminaManager ();
			anim.SetBool("PlayingHurt", PlayerActions.isHurt);

			if(cm) cm.ArtificialFixedUpdate();
			if(ladderClimbing) ladderClimbing.ArtificialUpdate();
			if(climbingWall) climbingWall.ArtificialUpdate();
			
			if(adv_groundChecker) adv_groundChecker.ArtificialUpdate();

			if(PlayerActions.objectPushing){
				GameManager.gameManager.cams[2].priority = 15;
				ObjectPushMech obm = GetComponent<ObjectPushMech>();
				obm.GrabControl();
			}else{
				GameManager.gameManager.cams[2].priority = 5;
			}

			if(Input.GetKeyDown(KeyCode.Y)){
				if(enableGUI)
					enableGUI = false;
				else
					enableGUI = true;
			}

			if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Busy") || anim.GetNextAnimatorStateInfo(0).IsTag("Busy"))
				PlayerActions.Busy = true;
			else
				PlayerActions.Busy = false;
			
			if(PlayerActions.Busy) anim.SetFloat ("MovementSpeed", 0, 0.1f, Time.deltaTime);
			float currentWeight = anim.GetLayerWeight(1);
			if (PlayerActions.isGrounded && PlayerActions.NotPerforming() && currentStamina > 10 && !PlayerActions.isAttacking && !PlayerActions.WalkActions())
			{
				if (Input.GetButton("Block"))
				{
					isBlocking = true;
					current_Shield_Property.Block();
					if(!PlayerActions.blockImpact) anim.SetLayerWeight (anim.GetLayerIndex("LeftArm"), Mathf.Lerp(currentWeight, 1, Time.deltaTime * 8)); else anim.SetLayerWeight (anim.GetLayerIndex("LeftArm"), 0);
				}
				else
				{
					isBlocking = false;
					current_Shield_Property.UnBlock();
					anim.SetLayerWeight (anim.GetLayerIndex("LeftArm"), Mathf.Lerp(currentWeight, 0, Time.deltaTime * 8));
				}
			}
			else
			{
				isBlocking = false;
				current_Shield_Property.UnBlock();
				anim.SetLayerWeight (anim.GetLayerIndex("LeftArm"), Mathf.Lerp(currentWeight, 0, Time.deltaTime * 8));
			}

			healthBar.fillAmount = playerHealth / playerMaxHealth;

			if(playerHealth > playerMaxHealth)
				playerHealth = playerMaxHealth;

			if(playerHealth <= 0){
				playerHealth = 0;
				Death();
			}else anim.SetBool("Dead", false);

	}
		
	public bool AliveEnemy(){
		bool enemy_near = Physics.CheckBox(transform.position + new Vector3(0, enemyChecker_Pos_Size.z, 0), new Vector3(enemyChecker_Pos_Size.x, enemyChecker_Pos_Size.y, enemyChecker_Pos_Size.x), Quaternion.identity, enemyLayer);
		if(enemy_near) return true; else return false;
	}
	public void TakeDamage(float Damage, int damageType, Transform target){
		if(isDead || PlayerActions.isDouging) return;
		anim.SetFloat("HurtType", 0);
		GameManager.gameManager.impulseSource.GenerateImpulseAt(transform.position, Vector3.one);


		if(!PlayerActions.Busy && !PlayerActions.objectPushing && !PlayerActions.Acrobatis() && damageType < 7){

			Vector3 lookDir = target.position - transform.position;
			lookDir.y = 0;
			transform.rotation  = Quaternion.LookRotation(lookDir);
			cm.addedVelocity = -transform.forward * knock_back_force;
		}

		
			
		PlayArmor("Action");

		if(isBlocking){
			if(currentStamina > damageType * 10){
				AudioCaller.MultiGroupAudioPlay(mainAudios, null, "Block", current_Shield_Property.shield_Mat);
				anim.SetFloat("HurtRate", damageType);
				float deduct_Stamind = (damageType * 10) / 5;
				currentStamina -= (damageType * 10) - (deduct_Stamind * current_Shield_Property.shield_Level);
			}else{
				anim.SetFloat("HurtRate", 4);
				GameManager.gameManager.StartCoroutine(GameManager.gameManager.SlowMotionTrigger(2f));
				isBlocking = false;
				currentStamina -= damageType * 10;
				AudioCaller.MultiGroupAudioPlay(mainAudios, null, "BlockBreak", current_Shield_Property.shield_Mat);
				playerHealth -= Damage / 2;
			}
			
			stamina_hold_period = 1;
			ParticleSystemManager.PlayParticle(ParticleSystemManager.ParticleList.blockSpark);
			anim.SetTrigger ("Impact");		
		}else{
			playerHealth -= Damage;
			ParticleSystemManager.PlayParticle(ParticleSystemManager.ParticleList.bloodParticle);			
			if(PlayerActions.isGrounded && !PlayerActions.Busy){ //isBusy checking was here!
				//if (!PlayerActions.isHurt && damageType < 7)
				//{
					
					anim.SetTrigger("Hurt");
					anim.SetFloat("HurtRate", damageType);
				//}
			}
		}
	}

	public void FallDamage(float Damage, bool play_anim)
	{
		if (playerHealth > 0)
		{
			playerHealth -= Damage;
			if(!play_anim) return;
			anim.SetTrigger("Hurt");
			anim.SetFloat("HurtRate", 5);
		}
	}

	private void Death(){

		if(!isDead){
			AudioCaller.SingleGroupAudioPlay(playerAudios, null ,"Die");
			anim.SetBool("Dead", true);
			isDead = true;
		}
	}

	void BodyDrop(){
		AudioCaller.SingleGroupAudioPlay(playerAudios, null,"BodyDrop");
		PlayArmor("Action");
		ParticleSystemManager.PlayParticle(ParticleSystemManager.ParticleList.groundDustParticle);
	}

	private void OnGUI(){

		if(enableGUI == true){	
			GUI.Box (new Rect (refField.x, refField.y + 25, refField.width, refField.height), "Busy:" + PlayerActions.Busy);
			GUI.Box (new Rect (refField.x, refField.y + 50, refField.width, refField.height), "ledgeClimbing:" + PlayerActions.ledgeClimbing);
			GUI.Box (new Rect (refField.x, refField.y + 75, refField.width, refField.height), "LedgeWalking:" + PlayerActions.ledgeWalking);
			GUI.Box (new Rect (refField.x, refField.y + 100, refField.width, refField.height), "Hurt:" + PlayerActions.isHurt);
			GUI.Box (new Rect (refField.x, refField.y + 125, refField.width, refField.height), "Attacking:" + PlayerActions.isAttacking);
			GUI.Box (new Rect (refField.x, refField.y + 175, refField.width, refField.height), "Dodging:" + PlayerActions.isDouging);
			GUI.Box (new Rect (refField.x, refField.y + 200, refField.width, refField.height), "LadderClimbing:" + PlayerActions.ladderClimbing);
			GUI.Box (new Rect (refField.x, refField.y + 225, refField.width, refField.height), "ObjectPushing:" + PlayerActions.objectPushing);
			GUI.Box (new Rect (refField.x, refField.y + 250, refField.width, refField.height), "isAiming:" + PlayerActions.isAiming);
			GUI.Box (new Rect (refField.x, refField.y + 275, refField.width, refField.height), "isGrounded:" + PlayerActions.isGrounded);
			GUI.Box (new Rect (refField.x, refField.y + 300, refField.width, refField.height), "EdgeSlip:" + PlayerActions.EdgeSlip());
			GUI.Box (new Rect (refField.x, refField.y + 325, refField.width, refField.height), "Acrobatics:" + PlayerActions.Acrobatis());
			GUI.Box (new Rect (refField.x, refField.y + 350, refField.width, refField.height), "NotPerforming:" + PlayerActions.NotPerforming());
			GUI.Box (new Rect (refField.x, refField.y + 375, refField.width, refField.height), "Sliding:" + PlayerActions.sliding);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube(transform.position + new Vector3(0, enemyChecker_Pos_Size.z, 0), new Vector3(enemyChecker_Pos_Size.x, enemyChecker_Pos_Size.y, enemyChecker_Pos_Size.x));
	}
	
	private void RootMotionHandler(){
		if(PlayerActions.RootMotionHandler()){
			anim.applyRootMotion = true;
		}else anim.applyRootMotion = false;
	}

	public void PlayArmor(string code){
		if(PlayerManager.Instance.armor_type != "") AudioCaller.MultiGroupAudioPlay(null, null, armor_type, code);
	}
}
