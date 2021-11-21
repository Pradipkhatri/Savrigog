using UnityEngine;
using System.Collections;

public class PushableBoxMovement : MonoBehaviour {

	private ObjectPushMech pushControls;

	public enum Directions{
		front, 
		back, 
		left,
		right,
	};

	public Directions direction;

	public bool checkerEnabled;

	[SerializeField] AudioClip clashAudio;

	void Start () {
		pushControls = PlayerManager.Instance.player.GetComponent<ObjectPushMech>();
		checkerEnabled = false;

	}

	void OnTriggerEnter (Collider other) {
	
			if(!other.CompareTag("Player") && !other.CompareTag("Ground")){
				checkerEnabled = true;
				AudioCaller.PlayOtherSound(clashAudio);

				switch(direction){
					case Directions.front:
						pushControls.ft = true;
						break;
					case Directions.back:
						pushControls.bk = true;
						break;
					case Directions.left:
						pushControls.lt = true;
						break;
					case Directions.right:
						pushControls.rt = true;
						break;
					default :
						break;
				}
			}
	}

	void OnTriggerExit(Collider other){

			checkerEnabled = false;

			switch(direction){
			case Directions.front:
				pushControls.ft = false;
				break;
			case Directions.back:
				pushControls.bk = false;
				break;
			case Directions.left:
				pushControls.lt = false;
				break;
			case Directions.right:
				pushControls.rt = false;
				break;
			default:
				break;
			}
	}
}
