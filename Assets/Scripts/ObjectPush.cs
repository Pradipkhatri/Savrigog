using UnityEngine;
using System.Collections;

public class ObjectPush : MonoBehaviour {

	[SerializeField] GUISkin GUIskin;

	GameObject pushAbleObject;
	[SerializeField] Transform wallCheckers;
	Transform player;
	Vector3 velocity;

	bool guiTriggerer = false;

	bool isGrabbing;

	void Start(){
		pushAbleObject = transform.parent.gameObject;
	}

	void OnTriggerExit(Collider other){
		if(other.gameObject.tag == "Player")
			guiTriggerer = false;
	}

	void OnTriggerStay (Collider other) {

		if(other.gameObject.tag == "Player"){

			if(isGrabbing){
				guiTriggerer = false;
				pushAbleObject.transform.parent = player;
				if(PlayerActions.Hurted() || PlayerActions.Busy){
					GrabFalse();
				}
			}else{
				guiTriggerer = true;
				pushAbleObject.transform.parent = null;
			}

			if(Input.GetButtonDown("Grab") ){
				if(!isGrabbing){
					if(PlayerActions.NotPerforming()){
						player = other.gameObject.transform;
						StartCoroutine(OnGrab(0.2f));
						wallCheckers.rotation = player.rotation;
						other.GetComponent<Animator>().SetBool("PushPosition", true);
						PlayerActions.objectPushing = true;
					}else return;
				}else{
					GrabFalse();
				}
			}
		}
	}

	IEnumerator OnGrab(float timer){
		while(timer > 0){
			player.position = Vector3.SmoothDamp(player.position, transform.position, ref velocity, 3 * Time.deltaTime);
			player.rotation = transform.rotation;
			timer -= Time.deltaTime;
			yield return null;
		}
		ParentSite();
	}

	void GrabFalse(){
		pushAbleObject.transform.parent = null;
		isGrabbing = false;
		PlayerActions.objectPushing = false; 
		player.GetComponent<Animator>().SetBool("PushPosition", false);
		
	}

	void ParentSite(){
		isGrabbing = true;
		pushAbleObject.transform.parent = player.transform;
	}

	void OnGUI(){
		GUI.skin = GUIskin;
		if(guiTriggerer)
			GUI.Box(new Rect(Screen.width/2, Screen.height - 70, 40, 40), "O");
	}

}
