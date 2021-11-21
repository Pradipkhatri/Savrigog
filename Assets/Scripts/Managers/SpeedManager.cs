using System.Collections;
using UnityEngine;
public class SpeedManager : MonoBehaviour
{
    [SerializeField] float min_stuckSpeed = 0.05f;
    [SerializeField] float max_stuckTime = 0.2f;
    private float current_stuckTime = 0.2f;
    
   void Start(){
       current_stuckTime = max_stuckTime;
   }
    void Update(){
        //anim.speed = Mathf.Clamp(anim.speed, 0, 1);

        if(PlayerManager.Instance.swordStuck){
            PlayerManager.Instance.anim.speed = min_stuckSpeed;
            current_stuckTime -= Time.deltaTime;
        }

        if(current_stuckTime <= 0){
            PlayerManager.Instance.swordStuck = false;
            PlayerManager.Instance.anim.speed = 1;
            current_stuckTime = max_stuckTime;
        }
    }

  

}