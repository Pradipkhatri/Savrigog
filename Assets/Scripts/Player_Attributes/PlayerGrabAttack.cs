using UnityEngine;
using System.Collections;

public class PlayerGrabAttack : MonoBehaviour {


//	PlayerAttack playerAttack;

	Animator anim;

	[SerializeField] Transform grabPoint;

	float grabTimer = 1;

	void Start () {
	//	playerAttack = GetComponent<PlayerAttack>();
		anim = PlayerManager.Instance.anim;
	}
	
	void FixedUpdate () {

		if (grabTimer > 0)
			grabTimer -= Time.deltaTime;

		if (PlayerActions.isGrounded && PlayerActions.NotPerforming()) {
			if (Input.GetButtonDown ("Grab") && grabTimer <= 0) {
				anim.SetTrigger ("Grab");
				grabTimer = 1;
			}
		}

		if (Input.GetButtonUp ("Grab")) {
			anim.ResetTrigger ("Grab");
		}
	}

	void Grab(){
		RaycastHit hit;

		Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
		Debug.DrawRay(transform.position + Vector3.up, transform.forward, Color.red);

		if(Physics.Raycast(ray, out hit, 2, 1 << 10)){
			EnemyAIUpdate enemyBehav = hit.transform.GetComponent<EnemyAIUpdate>();
			EnemyHealthManager enemyManager = hit.transform.GetComponent<EnemyHealthManager>();
			if(enemyManager.isAlive){
				anim.SetBool("Grabbed", true);
				//enemyBehav.hasbeenGrabbed = true;
				hit.transform.position = grabPoint.position;
				hit.transform.rotation = grabPoint.rotation;
			}
		}
	}

	void EndGrab(){
		anim.SetBool("Grabbed", false);
	}
}
