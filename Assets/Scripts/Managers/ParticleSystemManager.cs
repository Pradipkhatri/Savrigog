//using System.Collections;
using UnityEngine;

public static class ParticleSystemManager{

    public enum ParticleList
    {
        bloodParticle,
        groundDustParticle,
        groundSmash,
	blockSpark,
        arrowHit,
        slideDust,
        walkDust,
    };

    public static void PlayParticle(ParticleList particleList){
        ParticleSystem ps = GetParticle(particleList);
        if(ps.isPlaying) return; else ps.Play(true);
    }

    public static void StopParticle(ParticleList particleList){
        ParticleSystem ps = GetParticle(particleList);
        if(ps.isPlaying) ps.Stop(true); else return;
    }
    
    private static ParticleSystem GetParticle(ParticleList particleList){

	foreach(PlayerManager.ParticleSystems particleClip in PlayerManager.Instance.particleClipArray){
		if(particleClip.particleList == particleList){
			return particleClip.particlePrefab;
		}
	}
        Debug.LogError("ParticleSystem " + particleList + "NotFound");
        return null;
        }
}
