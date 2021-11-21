using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.ComponentModel;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour {
	
	bool canRun = true;

	[Header("WalkSpeed, RunSpeed, SprintSpeed")]
	[SerializeField] float[] wrs_movement_speed; //walk, run, sprint
	private float current_move_state = 1; //0 = walk, 1.1f = run, 2 = sprint

	public float current_rotate_speed;
	public Vector2 rotateSpeed; //current_speed, slow_rot, normal_rot

	[SerializeField]private float jumpHeight;
	[SerializeField]private float gravity = 20;
	public RaycastHit ground_hit;
	public float max_tilt = 0.07f;
	
	//Speed and velocity
	private float currentSpeed, speedSmoothVelocity;
	[SerializeField] float speedSmoothTime = 0.1f;

	public float velocityY;
	[HideInInspector] public Vector3 addedVelocity;

	//Components
	private Transform cam;
	private CharacterController controller;
	[HideInInspector] public float colliderHeight;
	[HideInInspector] public Vector3 colliderCenter;
	private Animator anim;
	private Vector3 Dir;
	float magnitude;
	float turnSmoothVelocity;
	//Give axis 

	private Vector2 moveInput;

	Vector3 velocity_before_air;
	[HideInInspector] public Quaternion ground_adjust_rotation;

	[HideInInspector] public bool edging;


	//Functions performed in Air.

	private bool greatFall = false;
	public float airTime = 0;
	[SerializeField]private float maxairTime = 0.6f;
	[SerializeField]float deadfall_time = 0.8f;
	[SerializeField] private float airControl = 4;

	AudioSource secoandary_audioSource;
	public bool flying = false;
	public int frame_rate = 60;
	private void Start () {
		cam = Camera.main.transform;
		secoandary_audioSource = GameManager.gameManager.secoandary_audioSource;
		controller = GetComponent<CharacterController> ();
		anim = PlayerManager.Instance.anim;
		colliderHeight = controller.height;
		colliderCenter = controller.center;
	}

	public void ArtificialUpdate(){
		if(PlayerActions.isGrounded && PlayerActions.NotPerforming())
		{
			if(!PlayerActions.Hurted()){
				if(!PlayerActions.WalkActions() && PlayerManager.Instance.currentStamina > 10   && PlayerManager.Instance.current_slope < controller.slopeLimit){
					if (moveInput.magnitude > 0  ){
						if(Input.GetButtonDown ("Dodge")) {
							transform.rotation = Quaternion.LookRotation(Dir);
							anim.SetTrigger ("Dodge");
						}
					}

					if(Input.GetButtonDown("Jump")){
						if(!PlayerActions.isAttacking){ 
							Jump();

						}
					}
				}
			} else return;
		}
/*
		AnimatorClipInfo an = anim.GetCurrentAnimatorClipInfo(0);
		if(PlayerActions.groundType == "Water"){
			 an.speed = 0.7f;
		}else an.speed = 1;*/
	}
	public void ArtificialFixedUpdate () {
		//Application.targetFrameRate = frame_rate;
		
		if(flying) return;

		if(PlayerManager.Instance.isDead || GameManager.gameManager.isPaused) return;


		if(Input.GetKey(KeyCode.V)){
			transform.position = Vector3.Lerp(transform.position, transform.forward, 2 * Time.deltaTime);
		}

		

		if (PlayerManager.Instance.currentStamina > 50)
			canRun = true;
		if(PlayerManager.Instance.currentStamina < 10)
			canRun = false;

		if(!PlayerActions.WalkActions()){
			if (Input.GetButton ("Boost") && PlayerManager.Instance.currentStamina > 5 && canRun) {
				PlayerActions.Sprinting = true;
				current_move_state = 2; //Current state is sprinting.
				if (anim.GetFloat ("MovementSpeed") > 1.5f) {
					PlayerManager.Instance.currentStamina -= Time.deltaTime * 25;
					PlayerManager.Instance.stamina_hold_period = 1.5f;
				}
			} else {
				PlayerActions.Sprinting = false;
				current_move_state = 1.1f; //Current state is running.
			}
		}else{
			current_move_state = 0;
			PlayerActions.Sprinting = false;
		}

		

		if (PlayerActions.SlowRotator()) current_rotate_speed = rotateSpeed.x; else current_rotate_speed = rotateSpeed.y;

		float jump_reach = Mathf.Sqrt(-2 * -gravity * jumpHeight);

		if(PlayerActions.Acrobatis()){
			airTime = 0;
			velocityY = 0;
		}else{
			velocityY -= 0.5f * gravity * Time.deltaTime;
		}
		
		if(PlayerActions.isGrounded) {
			velocity_before_air = new Vector3 (controller.velocity.x, 0, controller.velocity.z);
			velocityY = Mathf.Clamp(velocityY, -jump_reach, jump_reach);
			ParticleSystemManager.PlayParticle(ParticleSystemManager.ParticleList.walkDust);
		}else{
			PlayerActions.isDouging = false;
			ParticleSystemManager.StopParticle(ParticleSystemManager.ParticleList.walkDust);
		}
	
		//This is code to define the given keyboard axis.
		moveInput = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		anim.SetFloat("HorizontalSpeed", moveInput.x, speedSmoothTime, Time.deltaTime);
		anim.SetFloat("VerticalSpeed", moveInput.y, speedSmoothTime, Time.deltaTime);

		Vector3 forward = cam.transform.forward;
		Vector3 right = cam.transform.right;
		forward.y = 0;
		right.y = 0;
		forward.Normalize ();
		right.Normalize ();
						
		Dir = (forward * moveInput.y + right * moveInput.x).normalized;
		magnitude = Mathf.Clamp01 (moveInput.magnitude);
		if(PlayerActions.NotPerforming() && !PlayerActions.Hurted()) MovementAvailable(); //else OtherAction();
	}

	private void OtherAction(){
		
	}

	private void MovementAvailable(){

		if(PlayerActions.EdgeSlip()){
			addedVelocity = Vector3.Lerp(addedVelocity, Vector3.zero, 4 * Time.deltaTime);
			Vector3 additionalForce = addedVelocity;
			controller.Move (additionalForce * Time.deltaTime);
		}else addedVelocity = Vector3.zero;
		
		if(!PlayerActions.isAiming && !PlayerActions.Hurted() && PlayerManager.Instance.current_slope < controller.slopeLimit) PlayerRotation();
		
		//Calculating velocity for jump animation.
		float jumpT = velocityY / 100;
		anim.SetFloat("JumpTrigger", jumpT, 0.1f, Time.deltaTime);

		//Checking if the player is on ground or is on air...
		if(PlayerActions.isGrounded)
		{
			//anim.SetFloat("JumpTrigger", -1.0f);
			OnGround();
		}else{
			OnAir();
		}
	}

	private void PlayerRotation(){
		
		float target_rotation_y =  (Mathf.Atan2 (moveInput.x, moveInput.y) * Mathf.Rad2Deg + cam.eulerAngles.y);
		Vector3 rot = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, target_rotation_y, ref turnSmoothVelocity, current_rotate_speed);
		
		if(magnitude != 0){
			transform.eulerAngles = new Vector3(0, rot.y, 0);
		}else{
			transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
		}

		/*
		Quaternion face_rotation = Quaternion.identity;

		if(magnitude != 0){
			//float target_rotation_y =  (Mathf.Atan2 (moveInput.x, moveInput.y) * Mathf.Rad2Deg + cam.eulerAngles.y);
			//Vector3 rot = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, target_rotation_y, ref turnSmoothVelocity, current_rotate_speed);
			
			if(PlayerActions.isGrounded){
				if (!PlayerActions.Hurted() && PlayerManager.Instance.current_slope < controller.slopeLimit){
					//transform.eulerAngles = new Vector3(0, rot.y, 0);
					if(Dir != Vector3.zero) face_rotation = Quaternion.LookRotation(Dir);

					ground_adjust_rotation = Quaternion.FromToRotation(Vector3.up, ground_hit.normal);
					ground_adjust_rotation.x = Mathf.Clamp(ground_adjust_rotation.x, -max_tilt, max_tilt);
					ground_adjust_rotation.y = Mathf.Clamp(ground_adjust_rotation.y, -max_tilt, max_tilt);
					ground_adjust_rotation.z = Mathf.Clamp(ground_adjust_rotation.z, -max_tilt, max_tilt);

					Quaternion final_rot = face_rotation * ground_adjust_rotation;		

					transform.rotation = Quaternion.Lerp(transform.rotation, final_rot, current_rotate_speed * Time.deltaTime);
				}
			}else{
				transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
			}
		}*/

		/*
		Quaternion face_rotation = Quaternion.identity;
		Quaternion final_rot = Quaternion.identity;
		
		if(PlayerActions.isGrounded){					
			if (!PlayerActions.Hurted() && PlayerManager.Instance.current_slope < controller.slopeLimit) { //PlayerManager.Instance.controlAbleJump && !PlayerActions.isAiming &&
				
				if(moveInput.magnitude != 0){
					//ground_adjust_rotation = Quaternion.FromToRotation(Vector3.up, ground_hit.normal);

					//ground_adjust_rotation.x = Mathf.Clamp(ground_adjust_rotation.x, -max_tilt, max_tilt);
					//ground_adjust_rotation.z = Mathf.Clamp(ground_adjust_rotation.z, -max_tilt, max_tilt);

					if(Dir != Vector3.zero) face_rotation = Quaternion.LookRotation(Dir);

					//final_rot = face_rotation * ground_adjust_rotation;
					
					transform.rotation = Quaternion.Lerp(transform.rotation, face_rotation, current_rotate_speed * Time.deltaTime);
				}
			}
		}else{
			if(moveInput.magnitude != 0){
				transform.rotation = Quaternion.LookRotation(Dir);
				transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
			}else{
				transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0); //Changes done in rotation due to uneven ground is fixed.
			}
		}*/

	}

	private void Land()
	{
		GameManager.gameManager.impulseSource.GenerateImpulseAt(transform.position, Vector3.one);
		
		//Shaking camera on land.
		if(greatFall){
			if(airTime > deadfall_time) PlayerManager.Instance.FallDamage(PlayerManager.Instance.playerMaxHealth, true);
			if(airTime > maxairTime) PlayerManager.Instance.FallDamage(airTime * 150, true);
			airTime = 0;
			greatFall = false;
		}
		
		if(!PlayerActions.WalkActions()) ParticleSystemManager.PlayParticle(ParticleSystemManager.ParticleList.groundDustParticle);
		PlayerManager.Instance.PlayArmor("Action");
		AudioCaller.MultiGroupAudioPlay(null, secoandary_audioSource, "Land", PlayerActions.groundType);
	}

	private void OnGround(){
		if(airTime > 0){
			//transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
			//transform.rotation = ground_adjust_rotation * transform.rotation;

			if (airTime > maxairTime) greatFall = true;
			if (airTime > 0.03f) Land();
			airTime = 0;
		}
		if(!PlayerActions.isAttacking) Movement(); else OtherAction();;
		
	}

	private void Movement(){
		
		if(PlayerActions.WalkActions()) {
			anim.SetFloat("MoveType", 1, speedSmoothTime, Time.deltaTime);
		} else {
			anim.SetFloat("MoveType", 0, speedSmoothTime, Time.deltaTime);
		}
		
		

		float targetSpeed = wrs_movement_speed[Mathf.RoundToInt(current_move_state)] * magnitude; //Main Velocity Controller
		currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

		Vector3 jump_velocity = Vector3.up * velocityY;
		Vector3 velocity = Dir * currentSpeed + jump_velocity;
		
		controller.Move(velocity * Time.deltaTime);
		
		float new_s = Mathf.Clamp(current_move_state, 1, 5);
		
		anim.SetFloat("MovementSpeed", new_s * magnitude, speedSmoothTime, Time.deltaTime);
		
	}

	public void Jump(){
		addedVelocity = Vector3.zero;
		PlayerManager.Instance.PlayArmor("Action");
		AudioCaller.SingleGroupAudioPlay(null, secoandary_audioSource, "Jump");

		PlayerManager.Instance.currentStamina -= 10;
		PlayerManager.Instance.stamina_hold_period = 2;

		float jump_reach = (Mathf.Sqrt(-2 * -gravity * jumpHeight)) / 2;
		velocityY = jump_reach;
		velocityY += jump_reach;
		velocityY -= 0.5f * gravity * Time.deltaTime;
	}

	private void OnAir(){
		//velocity_before_air = Vector3.Lerp(velocity_before_air, Vector3.zero, Time.deltaTime * airControl);
		if(velocityY < 0) airTime += Time.deltaTime; else airTime = 0;

		if (airTime > maxairTime) anim.SetBool ("Fall", true); else anim.SetBool ("Fall", false);

		velocityY -= 0.5f * gravity * Time.deltaTime;
		
		Vector3 airVelocity;
		if (PlayerManager.Instance.controlAbleJump)
		{
			//airVelocity = Dir * wrs_movement_speed[Mathf.RoundToInt(current_move_state)] + Vector3.up * velocityY;
			airVelocity = (velocity_before_air + Dir) * airControl + Vector3.up * velocityY;
		}
		else
		{
			 airVelocity = Vector3.up * velocityY;
		}
		controller.Move(airVelocity * Time.deltaTime);
		//anim.SetFloat("MovementSpeed", 0f);
	}
	

	private void Dodge(int type){

		string dodge_type;
		if(type == 0) dodge_type = "Dodge"; else dodge_type = "Slide";

		AudioCaller.MultiGroupAudioPlay(null, secoandary_audioSource, dodge_type, PlayerActions.groundType);
		PlayerManager.Instance.PlayArmor("Action");

		controller.height = colliderHeight / 2;
		controller.center = new Vector3 (0, 2.2f, 0);

	}

	private void EndDodge(){
		AudioCaller.MultiGroupAudioPlay(null, secoandary_audioSource, "Land", PlayerActions.groundType);
		PlayerManager.Instance.PlayArmor("Action");
		controller.height = colliderHeight;
		controller.center = colliderCenter;
	}

	void Footstep( ){
		PlayerManager.Instance.PlayArmor("FootSteps");
		AudioCaller.MultiGroupAudioPlay(null, secoandary_audioSource, "FootSteps", PlayerActions.groundType);
	}
}
