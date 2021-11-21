using UnityEngine;
using System.Collections;

public class LedgeWalking : MonoBehaviour {

	AdvGroundChecker advGroundChecker;
	CharacterMovement cm;
	
	private Animator anim;

	private Vector3 velocity;
	private float fallTimer;

	private bool isOnLedge;
	private bool leftBalance;
	private bool rightBalance;
	private bool balance;

	public float maxFallTime = 5;
	
	void Start () {
		anim = PlayerManager.Instance.anim;
		cm = PlayerManager.Instance.cm;
		advGroundChecker = GetComponent<AdvGroundChecker>();
	}

	public void LedgeEnter(){

		leftBalance = advGroundChecker.leftLeg;
		rightBalance = advGroundChecker.rightLeg;
		balance = advGroundChecker.Balance;

		bool rightFall = false;

		if(leftBalance){
			fallTimer -= Time.deltaTime;
			rightFall = false;
		}

		if(rightBalance){
			fallTimer -= Time.deltaTime;
			rightFall = true;
		}

		if(balance){
			fallTimer = maxFallTime;
		}

		fallTimer = Mathf.Clamp(fallTimer, 0, maxFallTime);

		if(fallTimer <= 0){
			bool finalCondition = rightFall;
			Fall (finalCondition);
		}

	}

	private void Fall(bool fallRight){
		if(fallRight){
			cm.addedVelocity = transform.right * 2;
		}else{
			cm.addedVelocity = -transform.right * 2;
		}	
		
		fallTimer = maxFallTime;
	}
}
