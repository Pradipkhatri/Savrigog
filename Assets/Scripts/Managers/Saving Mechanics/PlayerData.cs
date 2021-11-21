using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
   public float kills;
   public float max_playerHealth;
   public float max_playerStamina;
   public float playerHealth;
   public float[] playerPosition;
   public int arrowCount;

   public PlayerData (PlayerManager player){
        kills = player.scoreStore;
        arrowCount = GameManager.gameManager.arrowCount;
        max_playerHealth = player.playerMaxHealth;
        max_playerStamina = player.maxStaminaLevel;
        playerHealth = player.playerMaxHealth;
        playerPosition = new float[3];
        playerPosition[0] = player.transform.position.x;
        playerPosition[1] = player.transform.position.y;
        playerPosition[2] = player.transform.position.z;
   }
}
