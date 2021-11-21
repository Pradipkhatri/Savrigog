using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbConformer : StateMachineBehaviour
{
    [SerializeField] string conform_to;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        IClimbed<string> climbed = PlayerManager.Instance.player.GetComponent<IClimbed<string>>();
        if(climbed != null){
            climbed.Climbed(conform_to);
        }
    }
}
