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

	SpwanManager spwanManager;
	void OnEnable(){
		if(spwanManager == null) spwanManager = SpwanManager.Instance;
		if(glowAble){
			swordMaterial.SetColor ("_EmissionColor", glowColor);
		}
	}

	public void ArtificialUpdate(){
		if(recoverTime > 0) {
			recoverTime -= Time.deltaTime;
			return;
		}

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
					StuckPlayer("ConcreteContact", true, hit);		
					break;
				case "Wood":
					PlayerActions.wallType = "Wood";
					StuckPlayer("WoodContact", true, hit);
					break;
				case "Metal":
					PlayerActions.wallType = "Metal";
					StuckPlayer("MetalContact", true, hit);
					break;
				case "Enemy":
					PlayerActions.wallType = "Enemy";
					EnemyFound(hit);
					break;
				default:
					PlayerActions.wallType = "Concrete";
					StuckPlayer("ConcreteContact", true, hit);
					break;
			}
			
			
		}
	}

	private void OnTriggerEnter(Collider other){
		if(other.CompareTag("Enemy") && PlayerManager.Instance.damageEnabled){
			EnemyHit(other.transform);
		}
	}

	void StuckPlayer(string parType, bool stuck, RaycastHit hit){
		
		if(stuckCount > 0 && stuck) 
		{
			PlayerManager.Instance.swordStuck = true;
			AudioCaller.MultiGroupAudioPlay(PlayerManager.Instance.swordAudios, null, weapon_type, PlayerActions.wallType);
			//Vector3 spwan_Location = hit.point + (hit.normal * 0.2f);

			GameObject particle = spwanManager.SpwanFromPool(parType, hit.point, Quaternion.LookRotation(hit.normal), true);

			GameManager.gameManager.impulseSource.GenerateImpulseAt(transform.position, Vector3.one * 2);
			stuckCount -= 1;
			recoverTime = contact_recoverTime;
		}

	}

	void EnemyHit(Transform enemy){
		PlayerManager.Instance.current_enemy = enemy;
		
		if(stuckCount > 0) {
			PlayerManager.Instance.swordStuck = true;
			GameManager.gameManager.impulseSource.GenerateImpulseAt(transform.position, Vector3.one * 2);
			stuckCount -= 1;
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
			if(enemyStats.bloodParticle_Tag != ""){
				GameObject particle = spwanManager.SpwanFromPool(enemyStats.bloodParticle_Tag, spwan_Location, Quaternion.LookRotation(hit.normal), true);
			}
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
