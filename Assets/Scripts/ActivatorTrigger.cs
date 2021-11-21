using UnityEngine;
using System.Collections;

public class ActivatorTrigger : MonoBehaviour {

	[SerializeField] Animation triggerAnimation;
	[SerializeField] AudioSource audioSource;
	[SerializeField] AudioClip audioClip;
	
	void Trigger()
	{
		triggerAnimation.Play();
		if (audioClip != null) audioSource.PlayOneShot(audioClip);
	}

}
