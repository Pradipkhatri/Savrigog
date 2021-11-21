using UnityEngine;
using System.Collections;

public class ObjectPushMech : MonoBehaviour {

	[SerializeField] float movementSpeed = 2;
	CharacterController cc;
	float currentSpeed;
	Animator anim;
	
	float speedSmoothVelocity;
	float speedSmoothTime = 0.1f;

	[HideInInspector] public bool ft, bk, lt, rt;
	Vector3 Dir;

	float inputX;
	float inputY;
	void Start () {
		cc = PlayerManager.Instance.cc;
		anim = PlayerManager.Instance.anim;
	}

	public void GrabControl(){
		Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		Vector3 forward = transform.forward;
		Vector3 right = transform.right;

		forward.y = 0;
		right.y = 0;

		forward.Normalize();
		right.Normalize();

		if(!ft || !bk){
			inputY = moveInput.y;
		} 

		if(!rt || !lt){
			inputX = moveInput.x;
		} 

		if(ft) {
			inputY = moveInput.y;
			inputY = Mathf.Clamp(inputY, -1, 0);
		}
		if(bk) {
			inputY = moveInput.y;
			inputY = Mathf.Clamp(inputY, 0, 1);
		}
		if(rt) {
			inputX = moveInput.x;
			inputX = Mathf.Clamp(inputX, -1, 0);
		}
		if(lt){
			inputX = moveInput.x;
			inputX = Mathf.Clamp(inputX, 0, 1);
		} 

		anim.SetFloat("HorizontalSpeed", inputX, 0.1f, Time.deltaTime);
		anim.SetFloat("VerticalSpeed", inputY, 0.1f, Time.deltaTime);

		Dir = (forward * inputY + right * inputX).normalized;

		currentSpeed = Mathf.SmoothDamp (currentSpeed, movementSpeed, ref speedSmoothVelocity, speedSmoothTime);
		Vector3 velocity = Dir * currentSpeed;
		
		cc.Move(velocity * Time.deltaTime);

	}
}
