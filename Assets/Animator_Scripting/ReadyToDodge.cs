using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyToDodge : StateMachineBehaviour {

	PlayerManager pm;
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(pm == null) pm = PlayerManager.Instance;
		PlayerActions.isDouging = true;
		pm.currentStamina -= 10;
		pm.stamina_hold_period = 2;
	}

	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//	PlayerActions.isDouging = true;
	//}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		PlayerActions.isDouging = false;
		pm.cc.height = pm.cm.colliderHeight;
		pm.cc.center = pm.cm.colliderCenter;
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
