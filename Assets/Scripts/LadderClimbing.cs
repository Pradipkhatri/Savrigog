using UnityEngine;
using System.Collections;

public class LadderClimbing : MonoBehaviour {
	
	private ClimbingWall climbingWall;
	bool climbingLadder = false;
	private Animator anim;
	//private CharacterController cc;
	float controllerAxis;


	[SerializeField] float climbSpeed = 5f;
	[SerializeField] Transform ladderChecker;
	[SerializeField] float checkDistance;

	bool foundLadder, climbingUp;

	Transform activeLadder;
	[SerializeField] Vector3 exitPosition;

	float timeEstimated = 0;

	void Start () {
		anim = PlayerManager.Instance.anim;
		climbingWall = GetComponent<ClimbingWall>();
		//cc = PlayerManager.Instance.cc;
	}

	public void ArtificialUpdate () {

		if(PlayerActions.isGrounded)
			climbingUp = false;

		PlayerActions.ladderClimbing = climbingLadder;

		float v = Input.GetAxis("Vertical");


		
		controllerAxis = v;
	
		anim.SetFloat("LadderClimb", controllerAxis, 0.1f, Time.deltaTime);

		if (PlayerActions.isHurt) {
			climbingLadder = false;
			this.transform.parent = null;
			timeEstimated = 0;
			anim.SetBool("ClimbingLadder", false);
		}

		if(!climbingUp)
			LadderCheck();

		if(foundLadder)
			LadderMount();

		LadderUnMount(v);

	}

	void LadderCheck(){
		RaycastHit hit;

		Ray ray = new Ray (ladderChecker.position, transform.forward);

		if(Physics.Raycast(ray, out hit, checkDistance)){
			if(hit.transform.tag == "Ladder"){
				foundLadder = true;
				activeLadder = hit.transform;
				//anim.SetFloat("JumpTrigger", 0.0f);
			}else{
				foundLadder = false;
				activeLadder = null;
			}
		}else
			foundLadder = false;

	}

	void LadderMount(){

		if(!foundLadder) {
			climbingLadder = false;
			LadderUnMount(-1);
			return;
		}

		if(!climbingLadder){

			if(Input.GetButtonDown("Grab")){
				climbingLadder = true;
			}

			if(!PlayerActions.isGrounded && PlayerManager.Instance.cm.velocityY > -5){ 
				climbingLadder = true;
			}
		}
		
		if(climbingLadder)
		{
			this.gameObject.GetComponent<CharacterMovement>().velocityY = 0;
			this.transform.parent = activeLadder.transform;
			transform.localEulerAngles = Vector3.zero;
			Vector3 finalPos = new Vector3(0, transform.localPosition.y, -0.6f);
		//	StartCoroutine(PlayerPlacing(0.2f, finalPos, false));
			transform.localPosition = finalPos;
			Climb(controllerAxis);
		}
		
	}

	
	void LadderUnMount(float v){
		if(v < 0){
			if(PlayerActions.isGrounded){
				UnMountProcess();
			}
		}

		if(PlayerActions.isGrounded && timeEstimated > 2f){
				UnMountProcess();
		}

		if(climbingLadder && !foundLadder && !PlayerActions.isGrounded && timeEstimated > 1){
			anim.SetBool("LadderExit", true);

			//if(v > 0){
				UnMountProcess();
			//}
		}

	}


	private void Climb(float V){
		if(PlayerActions.Busy) return;
		anim.SetBool("ClimbingLadder", true);
		timeEstimated += Time.deltaTime;

		if(V > 0 ){
			transform.Translate(Vector3.up * climbSpeed * Time.deltaTime);
		} 
		if(V < 0 ){
			transform.Translate(-Vector3.up * (climbSpeed + 2)* Time.deltaTime);
		}

	}
	private void UnMountProcess(){
			climbingLadder = false;
			this.transform.parent = null;
			anim.SetBool("ClimbingLadder", false);
			timeEstimated = 0;
	}
	public void Climbed(){
		climbingUp = false;
		UnMountProcess();
		anim.SetBool("LadderExit", false);
		transform.position = climbingWall.landPos * climbingWall.land_forward_multiplyer;
		PlayerActions.isGrounded = true;
		Invoke("ClimbConfromer", 0.1f);
	}

	void ClimbConformer(){
		anim.SetBool("OnWall", false);
	}
	
}
