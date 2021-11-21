using UnityEngine;
using System.Collections;

public class FollowChar : MonoBehaviour {


	public Transform posTarget;
	public Transform lookTarget;
	public Vector3 adjust;
	[SerializeField]private float Smooth = 2f;
	[SerializeField]private float followSpeed = 2f;
	[SerializeField]private float yfactor = 1.5f;
	private Vector3 offset;


	[SerializeField] bool changeRotation = false;
	[SerializeField] bool smoothFollow = false;
	void Start () {
		offset = (transform.position - posTarget.transform.position);
	}
	
	void Update () {


		Vector3 newPosition = posTarget.transform.position + offset + adjust;

		if(smoothFollow)
			transform.position = Vector3.Lerp(transform.position, newPosition, followSpeed * Time.deltaTime);
		else{
			transform.position = newPosition;
		}

		if(changeRotation){
			Vector3 Dir = (lookTarget.transform.position - transform.position) + new Vector3(0, yfactor, 0);
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation(Dir), Time.deltaTime * Smooth) ;
		}
	}


}
