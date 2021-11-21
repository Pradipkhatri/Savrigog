using UnityEngine;
using System.Collections;


public static class PlayerActions {

		public static bool isHurt, isAttacking, isDouging, ladderClimbing, ledgeClimbing, isAiming, Busy, objectPushing, Sprinting, blockImpact;		
		public static bool edging, wall_found, sliding;
		//Ground Checkers Requirements
		public static bool isGrounded, ledgeWalking;
		public static string groundType, wallType;


	public static bool NotPerforming(){
		if (isDouging || ladderClimbing || ledgeClimbing || objectPushing || Busy) {
			return false;
		} else
			return true;
	}
		
	public static bool Acrobatis(){
		if (ladderClimbing || ledgeClimbing) {
			return true;
		} else
			return false;
	}

	public static bool WalkActions(){
		if(isAiming || ledgeWalking)
			return true;
		else
			return false;
	}

	public static bool RootMotionHandler(){
		if(isHurt || Busy || isAttacking || isDouging || blockImpact){
			return true;
		}else return false;
	}

	public static bool Hurted(){
		if(isHurt || blockImpact){
			return true;
		}else return false;
	}

	public static bool SlowRotator(){
		if(isAttacking || !isGrounded || isDouging || Hurted() || ledgeWalking ){
			return true;
		}else return false;
	}

	public static bool EdgeSlip(){
		if(!isGrounded || Hurted() || ledgeWalking || edging){
			if(!Acrobatis()) return true; else return false;
		}else return false;
	}
}
