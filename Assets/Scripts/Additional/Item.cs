using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	public enum ItemType{
		HealthPack,
		ArrowPack,
	}
	
	private Transform target;
	private Transform mainCamera;

	[SerializeField] bool interactPick = true;

	[SerializeField] float destroyTime = 50;
	[SerializeField] int pickRange;
	[SerializeField] ItemType _itemType;
	[SerializeField] GameObject pickableIndicatorObject;
	SpriteRenderer sprite;
	Color spriteDefColor;
	
	[Tooltip("These objects will look at the cam constantly!")]
	[SerializeField] Transform[] additionalsShiners;
	
	float distance;

	[Tooltip("Enter the amount to be recovered!")]
	[SerializeField] int _amt = 50;
	//bool _picked = false;

	bool audioPlayed = false;

	void Start(){
		target = PlayerManager.Instance.player.transform;
		mainCamera = PlayerManager.Instance.mainCamera.transform;
		sprite = pickableIndicatorObject.GetComponent<SpriteRenderer>();
		spriteDefColor = sprite.color;
		pickableIndicatorObject.SetActive(false);
	}
	void Update(){
		distance = Vector3.Distance(transform.position, target.position);

		destroyTime -= Time.deltaTime;

		if(destroyTime < 10) {
			if(distance > pickRange) 
				sprite.color = new Color(0.7f, 0, 0, 0.7f);
			else
				sprite.color = spriteDefColor;
			pickableIndicatorObject.SetActive(true);
		}

		if(destroyTime < 0){
			Destroy(this.gameObject);
		}

		foreach(Transform a in additionalsShiners){
				a.LookAt(mainCamera.position);
		}

		if(distance < pickRange){
			if(interactPick){
				pickableIndicatorObject.SetActive(true);
				
				if(!audioPlayed){
					AudioCaller.SingleGroupAudioPlay(null, null, "ItemRange");
					audioPlayed = true;
				}

				if(Input.GetButtonDown("Grab")){
					PickUp();
				}
			}else PickUp();
		}else{
			audioPlayed = false;
			if(destroyTime > 10) pickableIndicatorObject.SetActive(false);
		}
	}
	
	void PickUp(){
			switch(_itemType){
				case ItemType.HealthPack:
					PlayerManager.Instance.playerHealth += _amt;
					break;
				case ItemType.ArrowPack:
					GameManager.gameManager.arrowCount +=  _amt;
					break;
				default:
					Debug.LogError("Please Select a Valid Operation");
					break;
			}
			AudioCaller.SingleGroupAudioPlay(null, null, "ItemPick");

			Destroy(this.gameObject);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, pickRange);
	}

}
