using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpwanTrigger : MonoBehaviour
{
    [SerializeField] EnemySpwan enemy_spwan;

    void OnTriggerEnter(Collider other){
        if(other.CompareTag("Player")){
            enemy_spwan.gameObject.SetActive(true);
            enemy_spwan.SpwannerTriggered(this.gameObject);
            this.gameObject.SetActive(false);
        }
    }
}
