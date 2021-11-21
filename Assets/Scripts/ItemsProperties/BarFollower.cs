using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BarFollower : MonoBehaviour {

	[SerializeField] Image Bar;
	[SerializeField] Image followerBar;
	[SerializeField] int smoothing = 2;
	[SerializeField] float delay = 0.5f;
	float delayTimer;
	float lerpTime;

	float previous_bar_value;
	float current_bar_value;
	void Update () {
		current_bar_value = Bar.fillAmount;

		if(previous_bar_value != current_bar_value){
			delayTimer = 0;
			lerpTime = 0;
			previous_bar_value = current_bar_value;
		}

		if(followerBar.fillAmount > Bar.fillAmount){
			delayTimer += Time.deltaTime;
			if(delayTimer > delay){
				lerpTime += Time.deltaTime;
				float percentComplete = lerpTime / smoothing;
				followerBar.fillAmount = Mathf.Lerp(followerBar.fillAmount, Bar.fillAmount, percentComplete);
			}
		}

		if(followerBar.fillAmount <= Bar.fillAmount){
			lerpTime = 0;
			delayTimer = 0;
			followerBar.fillAmount = Bar.fillAmount;
		}
	}
		
}
