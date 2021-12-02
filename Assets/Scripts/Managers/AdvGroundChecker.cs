using UnityEngine;
using System.Collections;

public class AdvGroundChecker : MonoBehaviour {

	[SerializeField] LayerMask groundLayers;
	[SerializeField] LayerMask ledgeLayers;

	private string groundType;

	[Space(10)]
	[Tooltip("Slippery of the character")]
	[SerializeField] float slipSpeed = 3;

	[SerializeField] float slide_maxSpeed = 6;
	[SerializeField] float maxSlide_acceleration = 2;

	private float slide_acceleration;
	public float slide_speed = 0;
	
	[SerializeField] float slope_limit_degree = 50;

	
	[Tooltip("Rays Position Maintainance")]
	[Header("Ray Pos")]
	[SerializeField] Vector3 ground_check_pointer_arrow = new Vector3(0.1f, 0.2f, 0.3f);
	[SerializeField] Vector2 m_GroundCheckPos = new Vector3(0.1f, 0.2f);

	[SerializeField] float new_fall_velocityY = -5; //Velocity while droping from ledge

	private bool isWalkingOnLedge = false;
	private bool isGrounded = true;

	[HideInInspector] public bool Balance = true, leftLeg = true, rightLeg = true;

	CharacterMovement cm;
	CharacterController cc;
	Animator anim;
	float slope_angle;
	Vector3 recent_slope;

	[SerializeField] AudioSource slide_audioSource;

	void Start(){
		anim = PlayerManager.Instance.anim;
		cc = PlayerManager.Instance.cc;
		cm = PlayerManager.Instance.cm;
		Balance = true;
		leftLeg = true;
		rightLeg = true;
	}

	public void ArtificialUpdate()
	{	   
		if (isGrounded){
			anim.SetBool ("OnGround", true);
			PlayerManager.Instance.controlAbleJump = true;
			//if(!isWalkingOnLedge && !PlayerActions.isHurt) PlayerActions.edging = false;
		}else{
			if(!PlayerActions.Acrobatis() && cm.velocityY < 0) SlipCheckers();
			anim.SetBool ("OnGround", false);
		}

		anim.SetBool("LedgeWalking", isWalkingOnLedge);

		MainGroundChecker();
	
		PlayerActions.isGrounded = isGrounded;
		PlayerActions.groundType = groundType;
		PlayerActions.ledgeWalking = isWalkingOnLedge;

		if(leftLeg && rightLeg) Balance = true;
	}

	private void MainGroundChecker(){
		RaycastHit hit;
		Vector3 ray_pos = cc.GetComponent<Collider>().transform.position;

		Ray ray = new Ray(ray_pos + (Vector3.up * m_GroundCheckPos.x), Vector3.down);
		Debug.DrawRay(ray_pos + (Vector3.up * m_GroundCheckPos.x), Vector3.down * m_GroundCheckPos.y, Color.green);
		
		if(Physics.Raycast(ray, out hit, m_GroundCheckPos.y, groundLayers + ledgeLayers)){
			PlayerManager.Instance.hit_normal = hit.normal;
			if(cm.velocityY < 0) isGrounded = true;
			cm.ground_hit = hit;
			if(hit.transform.tag == "Untagged") groundType = "Concrete"; else groundType = hit.transform.tag;
			
			slope_angle = Vector3.Angle(hit.normal, Vector3.up);

			if(slope_angle > PlayerManager.Instance.cc.slopeLimit && slope_angle < slope_limit_degree){
				SlopeSlip(hit);
				PlayerActions.sliding = true;
			}else{
				anim.SetFloat("Slope", 0, 0.1f, Time.deltaTime);
				PlayerActions.sliding = false;
				slide_speed= 0;
				if(slide_acceleration > 0.1f) {
					transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
					cc.Move(recent_slope * (slide_acceleration / 2));
					slide_acceleration -= Time.deltaTime * 2;
				}else {
					slide_audioSource.Stop();
					slide_acceleration = 0;
				}
			}

			if(hit.transform.CompareTag("WalkableLedge")){
				isWalkingOnLedge = true;
				GetComponent<LedgeWalking>().LedgeEnter();
				LedgeChecker();
			}else{
				isWalkingOnLedge = false;
			}
		}else{
			PlayerActions.sliding = false;
			slide_acceleration = 0;
			slide_speed = 0;
			if(!PlayerActions.Busy) isGrounded = false; else isGrounded = true;
			isWalkingOnLedge = false;
		}

		
		//slide_audioSource.volume = Mathf.Clamp(slide_acceleration, 0, 0.4f);
		//slide_audioSource.pitch = slide_acceleration;
		if(slide_acceleration < 0.1f) PlayerManager.Instance.current_slope = slope_angle; else PlayerManager.Instance.current_slope = cc.slopeLimit + 5;
		
	}

	
	private void SlopeSlip(RaycastHit hit){
		anim.SetFloat("Slope", slope_angle, 0.1f, Time.deltaTime);
		Vector3 slopeDirection = Vector3.up - hit.normal * Vector3.Dot(Vector3.up, hit.normal);
		if(slide_speed <= 0) {
			recent_slope = -slopeDirection;
			slide_audioSource.Play();
			slide_audioSource.time = Random.Range(0, slide_audioSource.clip.length);
			recent_slope.x = 0;
		}
		ParticleSystemManager.PlayParticle(ParticleSystemManager.ParticleList.slideDust);
		if(slide_speed < slide_maxSpeed)
			slide_speed += maxSlide_acceleration * Time.deltaTime;
		else	
			slide_speed= slide_maxSpeed;

		slide_acceleration = slide_speed/ slide_maxSpeed;

		Vector3 slide_dir = -slopeDirection * slide_speed + Vector3.down;
		PlayerManager.Instance.cc.Move(slide_dir * Time.deltaTime);
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-slopeDirection), 4 * Time.deltaTime);
	}

	private void SlipCheckers(){
		RaycastHit hit;
		Vector3 ray_spwan_pos = transform.position + Vector3.up * ground_check_pointer_arrow.y;

		Vector3 forward = transform.forward * ground_check_pointer_arrow.x + Vector3.up * ground_check_pointer_arrow.z;
		Vector3 back = -transform.forward * ground_check_pointer_arrow.x + Vector3.up * ground_check_pointer_arrow.z;
		Vector3 right = transform.right * ground_check_pointer_arrow.x + Vector3.up * ground_check_pointer_arrow.z;
		Vector3 left = -transform.right * ground_check_pointer_arrow.x + Vector3.up * ground_check_pointer_arrow.z;


		Ray front_ray = new Ray(ray_spwan_pos, forward);
		Ray back_ray = new Ray(ray_spwan_pos, back);
		Ray right_ray = new Ray(ray_spwan_pos, right);
		Ray left_ray = new Ray(ray_spwan_pos, left);

		float dis = ground_check_pointer_arrow.x;

		

		if(Physics.Raycast (front_ray, out hit, dis, groundLayers + ledgeLayers)){
			HitForSlip(-transform.forward);
		}

		if(Physics.Raycast (back_ray, out hit, dis, groundLayers + ledgeLayers)){
			HitForSlip(transform.forward);
		}

		if(Physics.Raycast (right_ray, out hit, dis, groundLayers + ledgeLayers)){
			HitForSlip(-transform.right);
		}

		if(Physics.Raycast (left_ray, out hit, dis, groundLayers + ledgeLayers)){
			HitForSlip(transform.right);
		}

	}

	void HitForSlip(Vector3 slip_direction){
		PlayerActions.edging = true;
		PlayerManager.Instance.controlAbleJump = false;
		cm.velocityY = new_fall_velocityY;
		cm.addedVelocity = slip_direction * slipSpeed;
	}

	#region Walkable Ledge
	private void LedgeChecker(){
		RaycastHit hit;

		Debug.DrawRay(transform.position + Vector3.up * 0.2f + transform.right * 0.1f, Vector3.down * m_GroundCheckPos.y, Color.magenta);
		Debug.DrawRay(transform.position + Vector3.up * 0.2f + -transform.right * 0.1f, Vector3.down * m_GroundCheckPos.y, Color.magenta);

		Ray leftRay = new Ray(transform.position + Vector3.up * 0.2f + transform.right * 0.1f, Vector3.down * m_GroundCheckPos.y);
		Ray rightRay = new Ray(transform.position + Vector3.up * 0.2f + -transform.right * 0.1f, Vector3.down * m_GroundCheckPos.y);

		if(leftLeg && rightLeg){
			Balance = true;
		} else Balance = false;
		
		if(Physics.Raycast(leftRay, out hit, m_GroundCheckPos.y, ledgeLayers)){
				leftLeg = true;
				if (Balance)
					anim.SetFloat ("LedgeBalance", 0, 0.1f, Time.deltaTime);
				else 
					anim.SetFloat ("LedgeBalance", 1, 0.5f, Time.deltaTime);
		}
		else
		{
			leftLeg = false;
		}

		if (Physics.Raycast (rightRay, out hit, m_GroundCheckPos.y, ledgeLayers)) {
				rightLeg = true;
				if (Balance)
					anim.SetFloat ("LedgeBalance", 0, 0.1f, Time.deltaTime);
				else 
					anim.SetFloat ("LedgeBalance", -1, 0.5f, Time.deltaTime);
		}else
		{
			rightLeg = false;
		}
	}
	#endregion

	void OnDrawGizmos(){
		Vector3 ray_spwan_pos = transform.position + Vector3.up * ground_check_pointer_arrow.y;

		Vector3 forward = transform.forward * ground_check_pointer_arrow.x + Vector3.up * ground_check_pointer_arrow.z;
		Vector3 back = -transform.forward * ground_check_pointer_arrow.x + Vector3.up * ground_check_pointer_arrow.z;
		Vector3 right = transform.right * ground_check_pointer_arrow.x + Vector3.up * ground_check_pointer_arrow.z;
		Vector3 left = -transform.right * ground_check_pointer_arrow.x + Vector3.up * ground_check_pointer_arrow.z;

		Gizmos.color = Color.red;
		Gizmos.DrawRay(ray_spwan_pos, forward);
		Gizmos.DrawRay(ray_spwan_pos, back);
		Gizmos.DrawRay(ray_spwan_pos, right);
		Gizmos.DrawRay(ray_spwan_pos, left);
	}
}
