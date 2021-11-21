using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{

    [SerializeField] AudioClip savedAudio;
    [SerializeField] AudioClip hover_Audio;
    [SerializeField] AudioClip cant_save;
    
    void OnTriggerEnter(Collider other){
        if(other.CompareTag("Player")){
           // PlayerManager.Instance.playerHealth = PlayerManager.Instance.playerMaxHealth;
            AudioCaller.PlayOtherSound(hover_Audio);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player")){
            PlayerManager.Instance.playerHealth += 1;
            if(Input.GetButtonDown("Grab")){
                if(!PlayerManager.Instance.AliveEnemy()){
                    Debug.Log("Game Saved");
                    GameManager.gameManager.SaveGame();
                    AudioCaller.PlayOtherSound(savedAudio);
                }else{
                    AudioCaller.PlayOtherSound(cant_save);
                    Debug.Log("Can't save while enemies are near");
                    return;
                } 
            }
        }
    }
}
