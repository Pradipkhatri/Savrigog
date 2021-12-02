using UnityEngine;
using System.Collections;

public class ObjectPush : MonoBehaviour {

	//[SerializeField] GUISkin GUIskin;

	Transform pushAbleObject;
	[SerializeField] Transform wallCheckers;
	Transform player;
	bool isGrabbing;

	bool process;

	void Start(){
		pushAbleObject = transform.parent.gameObject.transform;
		player = PlayerManager.Instance.player.transform;
	}

	void OnTriggerStay (Collider other) {
		if(process) return;
		if(other.gameObject.tag == "Player"){
			if(Input.GetButtonDown("Grab") ){
				if(isGrabbing){
					GrabFalse();
					process = true;
				}else{
					if(!PlayerActions.NotPerforming()) return;
					StartCoroutine(GrabTrue());
					process = true;
				}
				StartCoroutine(ProcessHandler());
			}
		}
	}

	IEnumerator ProcessHandler(){
		float t = 0.1f;
		while(t > 0){
			t -= Time.deltaTime;
			yield return null;
		}
		process = false;
	}

	IEnumerator GrabTrue(){
		player.GetComponent<Animator>().SetBool("PushPosition", true);
		PlayerActions.objectPushing = true;

		float timer = 0.2f;
		while(timer > 0){
			player.position = Vector3.Lerp(player.position, transform.position, 4 * Time.deltaTime);
			player.rotation = transform.rotation;
			timer -= Time.deltaTime;
			yield return null;
		}
		player.position = transform.position;
		player.rotation = transform.rotation;
		pushAbleObject.SetParent(player.transform);
		wallCheckers.rotation = transform.rotation;
		isGrabbing = true;
	}

	void GrabFalse(){
		pushAbleObject.SetParent(null);
		PlayerActions.objectPushing = false; 
		player.GetComponent<Animator>().SetBool("PushPosition", false);
		isGrabbing = false;
	}

}
