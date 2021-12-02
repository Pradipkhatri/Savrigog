using UnityEngine;
using System.Collections;

public class Lever : MonoBehaviour {

	[SerializeField] Animation selfAnimation;
	[SerializeField] float placement_Smoothing = 3;
	Animator anim;
	[SerializeField] bool hasPlayerAnimation;
	[SerializeField] Transform interactionPoint;
	[SerializeField] int playerAnimType;

	[SerializeField]AudioClip audioClip;

	[Tooltip("Should be greater than 0")]
	[SerializeField] float taskTime = 0.2f;
	Vector3 velocity;

	[Space(10)]
	[Header("Lever Prop")]
	[Space(10)]
	[SerializeField] Animation triggerAnimation;
	//[SerializeField] Transform triggerPos;
	[SerializeField] AudioClip triggeredAudio;
	void Start () {
		anim = PlayerManager.Instance.anim;
	}

	void Trigger(){
			if (triggerAnimation != null){
				triggerAnimation.Play();
				if(triggeredAudio != null){
					AudioCaller.PlayOtherSound(triggeredAudio);
				} 
			} 
	}

	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			
			if (Input.GetButtonDown("Grab") && !PlayerActions.objectPushing)
			{
				
				selfAnimation.Play();
				if(interactionPoint != null)
				{
					
					StartCoroutine(Adjustments(other.gameObject.transform, interactionPoint));
				}
				if (hasPlayerAnimation)
				{	anim.SetTrigger("Interact");
					anim.SetFloat("InteractType", playerAnimType);
				}
				
				AudioCaller.PlayOtherSound(audioClip);
				this.gameObject.GetComponent<BoxCollider>().enabled = false;
				
			}
		}
	}

	private IEnumerator Adjustments(Transform playerObj, Transform interactionPoint)
	{
		float taskTime_new = taskTime;
		while (taskTime_new > 0)
		{
			playerObj.position = Vector3.SmoothDamp(playerObj.position, interactionPoint.position, ref velocity, placement_Smoothing * Time.deltaTime);
			playerObj.rotation = interactionPoint.rotation;
			taskTime_new -= Time.deltaTime;
			yield return null;
		}
		playerObj.position = interactionPoint.position;
		playerObj.rotation = interactionPoint.rotation;
	}
}
