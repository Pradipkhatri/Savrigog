using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField] Text scoreText;
    public int scoreCount = 0;

    void Update(){
        scoreCount = Mathf.RoundToInt(PlayerManager.Instance.scoreStore);

        scoreText.text = "" + scoreCount;
    }
}
