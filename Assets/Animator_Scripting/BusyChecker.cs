using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusyChecker : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		PlayerActions.Busy = true;
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		PlayerActions.Busy = false;
	}

}
