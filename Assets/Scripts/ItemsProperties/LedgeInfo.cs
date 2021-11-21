using UnityEngine;
using System.Collections;
using System;

public class LedgeInfo : MonoBehaviour {

	public float holdPeriod = 0;
	public bool upJumpAble = false;
	public bool up_controlableJump = false;
	public bool climbAble = true;
	public bool jumpableLeft, jumpableRight;
	public int hangType = 1;
	public float climbSpeed = 1.5f;

	[Tooltip("X is 1, y is 2, Z is 3")]
	public int ledgeDir = 1; // Where 1 is x, 2 is y, and 3 is z.
	public Vector3 hangOffset = new Vector3 (0, -0.457f, -0.1113f);
	public Vector3 climbOffset = new Vector3(0, 1.341f, 0.356f);

	private void Update()
	{
        if (holdPeriod >= 0) holdPeriod -= Time.deltaTime;
		if (holdPeriod <= 0)
			holdPeriod = 0;
	}

	private void OnDrawGizmos(){
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere (transform.position, 0.3f);
	}

}
