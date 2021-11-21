using UnityEngine;
using System.Collections;
using AdvCameraShake;

public class CameraShaker : MonoBehaviour {

	public Vector2 camera_shake_rate = new Vector2(1, 1);

	void Update(){
		if(Input.GetKeyDown(KeyCode.L))
				GameManager.gameManager.ShakeCamera(camera_shake_rate.x, camera_shake_rate.y, transform.position);

	}

}
