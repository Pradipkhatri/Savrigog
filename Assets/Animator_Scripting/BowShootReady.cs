using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowShootReady : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		PlayerManager.Instance.shootReady = true;
	}

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		PlayerManager.Instance.shootReady = false;
	}

}
