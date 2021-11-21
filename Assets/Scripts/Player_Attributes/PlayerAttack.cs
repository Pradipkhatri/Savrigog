using UnityEngine;
using System.Collections;
using AdvCameraShake;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour {

	//private CharacterMovement characterMovement;
	[System.Serializable]
	public struct RightHandedWeapons
	{
		public string weaponName;
		public GameObject idleSword;
		public GameObject handSword;
	}


	private Animator anim;
	private WeaponProperties current_swordProperties;
	[SerializeField] int selected_swordID = 1;
	[SerializeField] private List<RightHandedWeapons> rightHandedWeapons = new List<RightHandedWeapons> ();

	[Space(25)]
	private GameObject idleSword;
	private GameObject handSword;

	[SerializeField] private float maxattackTimer = 1f;
	private float attackTimer;

	[SerializeField] private GameObject swordTrail;

	//public Vector3 attackRange = new Vector3(0.8f, 0.7f, 1);
	public LayerMask enemyLayer;

	bool damageEnabled = false;
	public Transform target_enemy;
	[SerializeField] int target_maxAngle = 45;
	public float current_targetAngle;

	AudioSource secoandary_audioSource;

	void Start() {
		anim = PlayerManager.Instance.anim;
		secoandary_audioSource = GameManager.gameManager.secoandary_audioSource;
		selected_swordID = -1;
		WeaponSelectionManager();
	}

	void WeaponSelectionManager(){
		int totalWeapons = rightHandedWeapons.Count - 1;
			
		AudioCaller.SingleGroupAudioPlay(null, secoandary_audioSource, "WeaponChange");

		selected_swordID++;

		if(selected_swordID > totalWeapons) selected_swordID = 0;
		
		idleSword = rightHandedWeapons [selected_swordID].idleSword;
		handSword = rightHandedWeapons [selected_swordID].handSword;
		current_swordProperties = handSword.GetComponent<WeaponProperties> ();
		PlayerManager.Instance.current_weapon_type = current_swordProperties.meleeWeapon.weapon_type;
		PlayerManager.Instance.RHS_weapon_Level = current_swordProperties.weaponLevel;
		if(current_swordProperties != null) swordTrail = current_swordProperties.meleeWeapon.sword_trail;
	}
		
	void SwordActivator()
	{

		if (PlayerActions.isAttacking) {
			idleSword.SetActive(false);
			handSword.SetActive(true);

		} else {
			if (anim.GetFloat ("MovementSpeed") > 0.5f || PlayerActions.objectPushing || PlayerActions.isGrounded) {
				idleSword.SetActive(true);
				handSword.SetActive(false);
			}
		}

		foreach (RightHandedWeapons wc in rightHandedWeapons) {
			if (selected_swordID != wc.handSword.GetComponent<WeaponProperties> ().weaponID) {
				wc.handSword.SetActive (false);
				wc.idleSword.SetActive (false);
			}
		}
			
	}
	
	void WeaponGlow(){
			
		if (PlayerActions.isAttacking) {
			current_swordProperties.meleeWeapon.swordMaterial.SetColor ("_EmissionColor", current_swordProperties.meleeWeapon.glowColor);
		} else {
			Color newColor = Color.Lerp (current_swordProperties.meleeWeapon.swordMaterial.GetColor("_EmissionColor"), new Color(0,0,0), 2 * Time.deltaTime);
			current_swordProperties.meleeWeapon.swordMaterial.SetColor ("_EmissionColor", newColor);
		}

	}

	private void EnemyTarget(){
		Transform current_enemy = PlayerManager.Instance.current_enemy;
		if(current_enemy == null) return;

		EnemyHealthManager ehm = current_enemy.GetComponent<EnemyHealthManager>();
		if(!ehm) return;

		Vector3 dir = current_enemy.transform.position - transform.position;
		current_targetAngle = Vector3.Angle(transform.forward, dir);

		if(ehm.isAlive && current_targetAngle < target_maxAngle){
			target_enemy = current_enemy;
		}else target_enemy = null;
	}
	
	public void ArtificialUpdate(){

		if (PlayerManager.Instance.isBlocking || PlayerActions.isAiming) return;

		PlayerManager.Instance.damageEnabled = damageEnabled;

		
		if (Input.GetButtonDown ("Fire1")) {

			if(PlayerManager.Instance.currentStamina > 10){
				attackTimer = maxattackTimer;
				anim.SetBool ("Attack", true);
				anim.SetBool ("HeavyAttack", false);
			}else
				AudioCaller.SingleGroupAudioPlay(null, secoandary_audioSource, "NoStamina");
		} else if (Input.GetButtonDown ("Fire2")) {
				if(PlayerManager.Instance.currentStamina > 10){
				attackTimer = maxattackTimer;
				anim.SetBool ("HeavyAttack", true);
				anim.SetBool ("Attack", false);
			}else
				AudioCaller.SingleGroupAudioPlay(null, secoandary_audioSource, "NoStamina");
		}
		


		EnemyTarget();

		if(idleSword != null || handSword != null) SwordActivator ();
		if (Input.GetButtonDown ("Toggle") && !PlayerActions.Busy && !PlayerActions.isAttacking) WeaponSelectionManager();


		if (attackTimer > 0 && PlayerActions.isGrounded) { 
			//Attacking
		}else{
			attackTimer = 0;
			anim.SetBool("Attack", false);
			anim.SetBool ("HeavyAttack", false);
			if(!PlayerActions.isAttacking) StopAttacking();
		}
		
		if (attackTimer > maxattackTimer) 
			attackTimer = maxattackTimer;

		if(current_swordProperties.meleeWeapon.glowAble) WeaponGlow ();
		current_swordProperties.meleeWeapon.ArtificialUpdate();

		attackTimer -= Time.deltaTime;
	}

	void GroundSmash(int range){
		if(!damageEnabled) damageEnabled = true;
		PlayerManager.Instance.currentStamina -= 10 * PlayerManager.Instance.damageType;
		PlayerManager.Instance.stamina_hold_period = 1.3f;
		PlayerManager.Instance.PlayArmor("Action");
		ParticleSystemManager.PlayParticle(ParticleSystemManager.ParticleList.groundSmash);
		AudioCaller.SingleGroupAudioPlay(null, null, "GroundSmash");
		PlayerManager.Instance.Smashed();

	}

	void DamagePoint() {
		if(!damageEnabled) damageEnabled = true;
		PlayerManager.Instance.currentStamina -= 10 * PlayerManager.Instance.damageType;
		PlayerManager.Instance.stamina_hold_period = 1.3f;
		
	}
	
	void StopAttacking() {
		attackTimer = 0;
		anim.SetBool("Attack", false);
		anim.SetBool ("HeavyAttack", false);
		if(damageEnabled) damageEnabled = false;
		if(swordTrail != null) swordTrail.SetActive (false);
	}

	void TrailStart(){
		PlayerManager.Instance.PlayArmor("FootSteps");
		current_swordProperties.meleeWeapon.ResetSwordStuck();
		AudioCaller.MultiGroupAudioPlay(PlayerManager.Instance.swordAudios, secoandary_audioSource, "Swoosh", current_swordProperties.meleeWeapon.swooshAudio);
		int swordPower = Mathf.RoundToInt( current_swordProperties.meleeWeapon.sword_power_curve.Evaluate (PlayerManager.Instance.damageType));
		PlayerManager.Instance.currentDamageRate = swordPower;
		if(swordTrail != null) swordTrail.SetActive (true);
		damageEnabled = true;
	}

	void TrailEnd(){
		if(swordTrail != null) swordTrail.SetActive (false);
		damageEnabled = false;
	}
}
