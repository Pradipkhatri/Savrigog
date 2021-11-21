using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyToDodge : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		PlayerActions.isDouging = true;
		PlayerManager.Instance.currentStamina -= 10;
		PlayerManager.Instance.stamina_hold_period = 2;
	}

	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//	PlayerActions.isDouging = true;
	//}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		PlayerActions.isDouging = false;
		CharacterMovement cm;
		cm = GameObject.FindGameObjectWithTag ("Player").GetComponent<CharacterMovement> ();
		PlayerManager.Instance.cc.height = cm.colliderHeight;
		PlayerManager.Instance.cc.center = cm.colliderCenter;
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
