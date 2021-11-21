using UnityEngine;
using System.Collections;

public class AxisFinder : MonoBehaviour {

	//We Made three variables.

	private Vector3 newestPos;
	public float refreshTime = 2f;
	public float forward;
	public float right;
	public Vector3 newPos;

	void Update () {

		//transform.hasdChanged is a bool that is triggred when a object changes it position.
		newestPos = transform.position;
		Vector3 currentPos = new Vector3(transform.position.x - 1, 0, transform.position.z - 1);


		float x = Mathf.Lerp(currentPos.x, newestPos.x, refreshTime * Time.deltaTime);
		float z = Mathf.Lerp(currentPos.z, newestPos.z, refreshTime * Time.deltaTime);

		newPos = new Vector3(x, 0, z);

		if(transform.hasChanged){
			if(transform.localPosition.z > newPos.z){
				forward = 1;
			}else if(transform.localPosition.z < newPos.z){
				forward = -1;
			}

		
			if(transform.localPosition.x > newPos.x){
				right = 1;
			}else if(transform.localPosition.x < newPos.x){
				right = -1;
			}

			//Be sure to set it to false after the workdone.

			transform.hasChanged = false;
		}else{
			forward = 0;
			right = 0;
			//newPos = transform.position;
		}



	}
}
