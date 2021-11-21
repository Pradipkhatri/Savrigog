using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public struct ChildArrowProperties{
    public float waitTime;
    public float followSpeed;
    //[HideInInspector] public ParentArrowProperties parent_arrow;
        
}

[System.Serializable] 
public class ParentArrowProperties{
    [HideInInspector] public ChildArrowProperties child_arrow;
    [HideInInspector] public Transform child_arrow_object;
    public LayerMask collisionLayers;
    public LayerMask enemyLayers;
    public float arrowSpeed = 50;   
    public float gravity = 0.05f; 
    public float fallTimer =0.2f;
    public int damageRate = 20;
    public float rayLength = 1f;
    public float sphereCast_Size = 0.28f;
    public AudioClip hitAudio;
    public ParticleSystem brustParticle;
    public ParticleSystem bloodParticle;
    public GameObject arrowHole;
}

[System.Serializable]
public class ArrowVar{
    public ArrowTypes arrow_Type;
    public Elements elements;
    public GameObject parent_arrow_object;
    public GameObject child_arrow_object;
    public int inventory;
}

[System.Serializable]
public enum ArrowTypes{
    Normal,
    Medium,
    Large,
}

[System.Serializable]
public enum Elements{
    Default,
    Fire,
    Ice,
    Poison,
    Magic,
}
