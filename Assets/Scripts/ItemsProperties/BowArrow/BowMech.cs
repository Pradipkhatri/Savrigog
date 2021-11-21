using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowMech : MonoBehaviour
{
    public ArrowTypes acceptable_arrows;

    [Space(10)]
    public GameObject hand_object;
    public GameObject show_object;

    public float charge_speed = 2;
    public int current_charge_state;
    public int power_multiplyer = 1;

    public float max_recover_speed = 1;

    [Header("Bullet_Options")]
    public Transform spwan_loc;
    public GameObject arrow_object;

    [Header("AudioClips")]
    public AudioClip _stretchAudio;
    public AudioClip _shootAudio;
}

