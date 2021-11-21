using UnityEngine;
using System.Collections;

public class IKFootController : MonoBehaviour {

	private Animator anim;

	private Vector3 rightFootPosition, leftFootPosition, leftFootIKPosition, rightFootIKposition;
	private Quaternion leftFootIKRotation, rightFootIKRotation;
	 private float lastPelvisPositionY, lastRightFootPositionY, lastLeftFootPositionY;

	[Header("FeetIK Properties")]
	[SerializeField] private bool enableIK = true;
	[SerializeField]  [Range(0, 2)]private float heightFromGroundRayCast = 1.14f;
	[SerializeField] [Range(0, 2)] private float raycastDownDistance = 1.5f;
	[SerializeField] private float pelvisOffset = 0f;
	[SerializeField] [Range(0, 1)] private float pelvisUpDownSpeed = 0.28f;
	[SerializeField] [Range(0, 1)] private float feet_to_IK_positionSpeed = 0.5f;

	[SerializeField] string leftFootAnimVariableName = "LeftFootCurve";
	[SerializeField] string rightFootAnimVariableName = "RightFootCurve";

	public bool useProIKFeature = false;
	public bool showSolverDebug = true;
	

	void Start () {
		anim = PlayerManager.Instance.anim;
	}

	///<summary>
	///We are updating the AdjustFeetTarget method and also find the position of each foot inside our solver position.
	///</summary>
	private void FixedUpdate(){
		if(!enableIK || !anim) return;

		AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
		AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

		//find and raycast to the ground to find positions.
		FeetPositionSolver(rightFootPosition, ref rightFootIKposition, ref rightFootIKRotation); // Handle the solver for right foot.
		FeetPositionSolver(leftFootPosition, ref leftFootIKPosition, ref leftFootIKRotation); // Handle the solver for left foot.
	}
	

	void OnAnimatorIK (int layerIndex) {
		if(!enableIK || !anim) return;
		MovePelvisHeight();

		//Right foot IK position and rotation -- utilize the pro features in here!
		anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

		if(useProIKFeature){
			anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat(rightFootAnimVariableName));
		}

		MoveFeetToIKPoint(AvatarIKGoal.RightFoot, rightFootIKposition, rightFootIKRotation, ref lastRightFootPositionY);

		//Left foot IK position and rotation -- utilize the pro features in here!
		anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

		if(useProIKFeature){
			anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat(leftFootAnimVariableName));
		}

		MoveFeetToIKPoint(AvatarIKGoal.LeftFoot, leftFootIKPosition, leftFootIKRotation, ref lastLeftFootPositionY);
	}

	void MoveFeetToIKPoint (AvatarIKGoal foot, Vector3 positionIKHolder, Quaternion rotationIKHolder, ref float lastFootPositionY){
		Vector3 targetIKposition = anim.GetIKPosition(foot);
		if(positionIKHolder != Vector3.zero){
			targetIKposition = transform.InverseTransformPoint(targetIKposition);
			positionIKHolder = transform.InverseTransformPoint(positionIKHolder);

			float yVariable = Mathf.Lerp(lastFootPositionY, positionIKHolder.y, feet_to_IK_positionSpeed);
			targetIKposition.y += yVariable;

			lastFootPositionY = yVariable;
			targetIKposition = transform.TransformPoint(targetIKposition);
			anim.SetIKRotation(foot, rotationIKHolder);
		}

		anim.SetIKPosition(foot, targetIKposition);
	}

	void MovePelvisHeight(){
		if(rightFootIKposition == Vector3.zero || leftFootIKPosition == Vector3.zero || lastPelvisPositionY == 0){
			lastPelvisPositionY = anim.bodyPosition.y;
			return;
		}

		float lOffsetPosition = leftFootPosition.y - transform.position.y;
		float rOffsetPosition = rightFootIKposition.y - transform.position.y;

		float totalOffset = (lOffsetPosition < rOffsetPosition) ? lOffsetPosition : rOffsetPosition;

		Vector3 newPelvisPosition = anim.bodyPosition + Vector3.up * totalOffset;
		newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, pelvisUpDownSpeed);
		anim.bodyPosition = newPelvisPosition;
		lastPelvisPositionY = anim.bodyPosition.y;
	}


	//Using Raycast to find feet position and solving
	void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIKPositions, ref Quaternion feetIKRotations){
		RaycastHit hit;
	
		if(showSolverDebug)
			Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (raycastDownDistance + heightFromGroundRayCast), Color.yellow);
		
		if(Physics.Raycast(fromSkyPosition, Vector3.down, out hit, raycastDownDistance + heightFromGroundRayCast, GameManager.gameManager.groundLayers)){
			feetIKPositions = fromSkyPosition;
			feetIKPositions.y = hit.point.y + pelvisOffset;
			feetIKRotations = Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation;

			return;
		}
		
		feetIKPositions = Vector3.zero;

	}

	void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot){
		feetPositions = anim.GetBoneTransform(foot).position;
		feetPositions.y = transform.position.y + heightFromGroundRayCast;
	}
}
