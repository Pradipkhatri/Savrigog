using UnityEngine;
using System.Collections;
using AdvCameraShake;

public class WallClimbing : MonoBehaviour
{
	private CharacterMovement cm;
	

	private Animator anim;
	private Transform player;

	public Transform[] ledgeCheckers;


	[Header("Essentials")]
	[SerializeField] LayerMask ledgeLayer;
	[SerializeField] float climbSpeed = 2;
	[SerializeField] float checkRange = 0.2f;
	float h;
	float v;
	bool onLedge = false;
	bool active_LedgeCheckers = true;

	LedgeInfo ledgeInfo;

	void Start()
	{
		cm = PlayerManager.Instance.cm;
		anim = PlayerManager.Instance.anim;
		player = PlayerManager.Instance.player.transform;
	}

	void Update()
	{

		h = Input.GetAxisRaw("Horizontal");
		v = Input.GetAxisRaw("Vertical");

		if (anim == null)
		{
			anim = PlayerManager.Instance.anim;
		}

		if(!onLedge){
			anim.SetFloat("WallClimbSide", h, 0.1f, Time.deltaTime);
		}

		if (!PlayerActions.isGrounded)
		{
			if(active_LedgeCheckers) LedgeChecker();


			if (onLedge)
				IsClimbing();
			//Inputs were here.
		}
		else
		{
			onLedge = false;
			active_LedgeCheckers = true;
			PlayerActions.ledgeClimbing = false;
			anim.SetBool("OnWall", false);
			anim.SetBool("ClimbUp", false);
		}



	}

	/*//Side Ledge Checkers
	private IEnumerator SideLedgeCheckers(){


		while (!AdvGroundChecker.Instance.isGrounded) {
			RaycastHit hit;

			Ray ray2 = new Ray (ledgeCheckers [0].position, transform.right);
			Ray ray3 = new Ray (ledgeCheckers [0].position, -transform.right);

			if (Physics.Raycast (ray2, out hit, checkRange, ledgeLayer) || Physics.Raycast (ray3, out hit, checkRange, ledgeLayer)) {
				if (cm.velocityY <= 2) {
						ledgeInfo = hit.transform.gameObject.GetComponent<LedgeInfo> ();

						if (ledgeInfo != null) {
						if (ledgeInfo.holdPeriod <= 0f){
								WallWalk (hit.transform, ledgeInfo.hangType);
						}

					}
				}
			}

			yield return null;
		}
	}*/


	private void LedgeChecker()
	{		
	

		RaycastHit hit;
		Ray ray = new Ray(ledgeCheckers[0].position, transform.forward);

		if (Physics.Raycast (ray, out hit, checkRange, ledgeLayer)) {
			if (cm.velocityY <= 2) {
					ledgeInfo = hit.transform.gameObject.GetComponent<LedgeInfo> ();
						
				if (ledgeInfo != null) {
					if (ledgeInfo.holdPeriod <= 0f){
							onLedge = true;
							WallWalk (hit.transform, ledgeInfo.hangType);
					}
				}
					
			}
		} else
			return;

	}
	
	#region LedgeClimbing
	
	private void WallWalk(Transform ledge, int hangType)
	{
		if (ledge == null)
			return;


		anim.SetBool("OnWall", true);
		onLedge = true;
		player.transform.parent = ledge;
		PlayerActions.ledgeClimbing = true;
		player.localEulerAngles = Vector3.zero;
		GameManager.gameManager.impulseSource.GenerateImpulse(1);
		AudioCaller.SingleGroupAudioPlay(null, null, "GrabLedge");
		Vector3 finalPos = new Vector3(transform.localPosition.x, ledgeInfo.hangOffset.y, ledgeInfo.hangOffset.z);
		transform.localPosition = finalPos;
	}

	/*
		if (hangType == 1)
		{
			switch (ledgeInfo.ledgeDir) {
				case 1:
					player.localPosition = new Vector3 (transform.localPosition.x, ledgeInfo.hangOffset.y, ledgeInfo.hangOffset.z);
					break;
				case 2:
					player.localPosition = new Vector3 (ledgeInfo.hangOffset.x, transform.localPosition.y, ledgeInfo.hangOffset.z);
					break;
				case 3:
					player.localPosition = new Vector3 (ledgeInfo.hangOffset.x, ledgeInfo.hangOffset.y, transform.localPosition.z);
					break;
				default:
					Debug.LogError("Please verify the direction");
					break;
			}
		}
		}
		*/
	
	
	private void IsClimbing()
	{
		climbSpeed = ledgeInfo.climbSpeed;
		CharacterMovement cm = this.GetComponent<CharacterMovement>();
		cm.velocityY = 0f;
		active_LedgeCheckers = false;
	
		if (!anim.GetBool("ClimbUp") && onLedge)
		{
			//if(!PlayerActions.Busy)
			//////////ClimbSideWalk();
			if (Input.GetButtonDown("Jump") && v < 0)
			{
				ledgeInfo.holdPeriod = 0.3f;
				active_LedgeCheckers = true;
				player.transform.parent = null;
				PlayerActions.ledgeClimbing = false;
				anim.SetBool("OnWall", false);
				onLedge = false;
				cm.addedVelocity = -transform.forward;
				player.transform.rotation = Quaternion.LookRotation(transform.forward);
				PlayerManager.Instance.controlAbleJump = false;
			}
			
		}

		if (Input.GetButtonDown("Jump") && v > 0 && h == 0)
		{
			ledgeInfo.holdPeriod = 0.3f;
			if (ledgeInfo.climbAble)
			{
				anim.SetBool("ClimbUp", true);
				onLedge = false;
				player.transform.parent = null;
				//active_LedgeCheckers = true;
			}
			else if (ledgeInfo.upJumpAble)
			{
				if(!ledgeInfo.up_controlableJump){
					PlayerManager.Instance.controlAbleJump = false;
				}
				player.transform.parent = null;
				//active_LedgeCheckers = true;
				PlayerActions.ledgeClimbing = false;
				anim.SetBool("OnWall", false);
				onLedge = false;
				cm.velocityY = 16;
				player.transform.rotation = Quaternion.LookRotation(transform.forward);
			}else return;
		}

	}

	private void Climbed()
	{
		transform.parent = null;
		anim.SetBool("ClimbUp", false);
		anim.SetBool("OnWall", false);
		PlayerActions.ledgeClimbing = false;


		Vector3 newOffset = ledgeInfo.climbOffset;
		Vector3 newPosition = transform.position + newOffset;
		transform.position = newPosition;
		PlayerActions.isGrounded = true;
			//PlayerManager.Instance.pauseJump = 0.14f;
		
	}

	private void ClimbSideWalk()
	{
		RaycastHit hit;

		Ray ray1 = new Ray(ledgeCheckers[1].position + -transform.forward * 0.1f, transform.forward);
		Ray ray2 = new Ray(ledgeCheckers[2].position + -transform.forward * 0.1f, transform.forward);

		Debug.DrawRay(ledgeCheckers[1].position + -transform.forward * 0.1f, transform.forward * checkRange);
		Debug.DrawRay(ledgeCheckers[2].position + -transform.forward * 0.1f, transform.forward * checkRange);

		if (Physics.Raycast(ray1, out hit, checkRange, ledgeLayer))
		{
			if (h < 0) {
				if (onLedge) {
					float targetSpeed = h * -climbSpeed;
					transform.Translate(-Vector3.right * targetSpeed * Time.deltaTime);
					anim.SetFloat ("WallClimbSide", -1, 0.1f, Time.deltaTime);
				}
			}

		}
		else
		{
			if(onLedge){
				if (h < 0 && ledgeInfo.jumpableLeft)
				{
					anim.SetFloat("WallClimbSide", -2, 0.1f, Time.deltaTime);

					if (Input.GetButtonDown("Jump"))
					{
						ledgeInfo.holdPeriod = 0.3f;
						//StartCoroutine (SideLedgeCheckers());
						active_LedgeCheckers = true;
						player.transform.parent = null;
						PlayerActions.ledgeClimbing = false;
						anim.SetBool("OnWall", false);
						onLedge = false;
						cm.velocityY = 12;
						cm.addedVelocity = -transform.right * 10;
						PlayerManager.Instance.controlAbleJump = false;
						player.transform.rotation = Quaternion.LookRotation(-transform.right);

					}
				}else{
					anim.SetFloat("WallClimbSide", 0, 0.1f, Time.deltaTime);
				}
			}else
				return;
		}


		if (Physics.Raycast(ray2, out hit, checkRange, ledgeLayer))
		{
			if (h > 0)
			{
				if (onLedge) {
					float targetSpeed = h * climbSpeed;
					transform.Translate(Vector3.right * targetSpeed * Time.deltaTime);
					anim.SetFloat ("WallClimbSide", 1, 0.1f, Time.deltaTime);
				} 
			}
		}
		else
		{
			if(onLedge){
				if (h > 0 && ledgeInfo.jumpableRight) {
					anim.SetFloat ("WallClimbSide", 2, 0.1f, Time.deltaTime);

					if (Input.GetButtonDown ("Jump")) {
						ledgeInfo.holdPeriod = 0.3f;
						//StartCoroutine (SideLedgeCheckers());
						active_LedgeCheckers = true;
						player.transform.parent = null;
						PlayerActions.ledgeClimbing = false;
						anim.SetBool ("OnWall", false);
						onLedge = false;
						cm.velocityY = 12;
						cm.addedVelocity = transform.right * 10;
						PlayerManager.Instance.controlAbleJump = false;
						player.transform.rotation = Quaternion.LookRotation (transform.right);

					}
				} else {
					anim.SetFloat ("WallClimbSide", 0, 0.1f, Time.deltaTime);
				}
			}else
				return;
		}

		if (h == 0) {
			anim.SetFloat ("WallClimbSide", 0, 0.1f, Time.deltaTime);
		}
				

	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawRay(ledgeCheckers[0].position, transform.forward * checkRange);
		Gizmos.DrawRay(ledgeCheckers[0].position, transform.right * checkRange);
		Gizmos.DrawRay(ledgeCheckers[0].position, -transform.right * checkRange);
	}
	#endregion

}
