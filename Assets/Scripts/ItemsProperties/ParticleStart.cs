using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleStart : MonoBehaviour {

	[SerializeField] AudioClip startAudio;
	[SerializeField] ParticleSystem particle_System;
	[SerializeField] ParticleSystem counterParticle;
	//[SerializeField] GameObject sword;
	//[SerializeField] Material swordMaterial;
	
	void OnEnable () {
		AudioCaller.PlayOtherSound (startAudio);
		if(particle_System != null) particle_System.Play (true);
		//swordMaterial.SetColor("_EmissionColor", new Color(188, 185, 88) * 2);
		if(counterParticle != null) counterParticle.Play (true);
	}

	void OnDisable(){
		//swordMaterial.SetColor("_EmissionColor", Color.black);
		if(particle_System != null) particle_System.Stop (true);
	}

}
