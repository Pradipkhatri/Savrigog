using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
   	public bool glowAble;
    [HideInInspector] public int weaponLevel;
    public Material swordMaterial;
	public Color glowColor;
	public AnimationCurve sword_power_curve;
	public GameObject sword_trail;
	//[SerializeField] float ray_expand = 0.1f;
	
	int stuckCount = 2;
	[SerializeField] int max_sword_stuck_count = 2;

	[SerializeField] Transform playerShooter_empty, swordReceiver_empty;	
	float recoverTime = 0.1f;
	[Tooltip("This time will create a gap while playing the swordContact part!")]	
	[SerializeField] float contact_recoverTime = 0.1f;
	[SerializeField] LayerMask hitLayers;
	public string swooshAudio = "Medium";
	public string weapon_type = "Sword";

	void OnEnable(){
		if(glowAble){
			swordMaterial.SetColor ("_EmissionColor", glowColor);
		}
	}

	public void ArtificialUpdate(){
		if(recoverTime > 0) recoverTime -= Time.deltaTime;
		if(PlayerManager.Instance.damageEnabled && !PlayerManager.Instance.isDead){
			ShootRay();
		}
	}	

	private void ShootRay(){
		RaycastHit hit;

		if(Physics.Linecast(playerShooter_empty.position, swordReceiver_empty.position, out hit, hitLayers)){

			switch(hit.transform.tag){
				case "Concrete":
					PlayerActions.wallType = "Concrete";
					StuckPlayer(0, true, hit);		
					break;
				case "Wood":
					PlayerActions.wallType = "Wood";
					StuckPlayer(1, true, hit);
					break;
				case "Metal":
					PlayerActions.wallType = "Metal";
					StuckPlayer(2, true, hit);
					break;
				case "Enemy":
					PlayerActions.wallType = "Flesh";
					EnemyFound(hit);
					break;
				default:
					PlayerActions.wallType = "Concrete";
					break;
			}
			
			
		}
	}

	private void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "Enemy" && PlayerManager.Instance.damageEnabled){
			EnemyHit(other.transform);
		}
	}

	void StuckPlayer(int parType, bool stuck, RaycastHit hit){
		if(recoverTime > 0) return;
		if(stuckCount > 0 && stuck) 
		{
			PlayerManager.Instance.swordStuck = true;
			AudioCaller.MultiGroupAudioPlay(PlayerManager.Instance.swordAudios, null, weapon_type, PlayerActions.wallType);
			Vector3 spwan_Location = hit.point + (hit.normal * 0.2f);
			Instantiate(GameManager.gameManager.swordContactParticles[parType], spwan_Location, Quaternion.LookRotation(hit.normal));
			GameManager.gameManager.impulseSource.GenerateImpulseAt(transform.position, Vector3.one);
			stuckCount -= 1;
			recoverTime = contact_recoverTime;
		}

	}

	void EnemyHit(Transform enemy){
		PlayerManager.Instance.current_enemy = enemy;
		

		if(recoverTime <= 0){
			if(stuckCount > 0) {
				PlayerManager.Instance.swordStuck = true;
				GameManager.gameManager.impulseSource.GenerateImpulseAt(transform.position, Vector3.one * 2);
				stuckCount -= 1;
			}
		}

		IDamageable damageable = enemy.GetComponent<IDamageable>();
		if(damageable == null) return;
		int swordPower = Mathf.RoundToInt( sword_power_curve.Evaluate (PlayerManager.Instance.damageType));
		damageable.TakeDamage (PlayerManager.Instance.damageType, weaponLevel * swordPower, true);
		recoverTime = contact_recoverTime;
	}

	void EnemyFound(RaycastHit hit){

		Vector3 spwan_Location = hit.point + (hit.normal * 0.2f);
		EnemyHealthManager enemyStats = hit.transform.GetComponent<EnemyHealthManager>();
		if(recoverTime <= 0){
			if(enemyStats.bloodParticle != null) 
				Instantiate(enemyStats.bloodParticle, spwan_Location, Quaternion.LookRotation(hit.normal));
		}	
	}

	public void ResetSwordStuck(){
		stuckCount = max_sword_stuck_count;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		if(swordReceiver_empty != null) {
			Gizmos.DrawLine(playerShooter_empty.position, swordReceiver_empty.position);
		}
	}
}
