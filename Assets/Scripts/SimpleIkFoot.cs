using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleIkFoot : MonoBehaviour {

    Animator anim;

    [SerializeField] string left_foot_ik_parameter;
    [SerializeField] string right_foot_ik_parameter;

    [SerializeField] LayerMask layerMask; // Select all layers that foot placement applies to.

    [Range (0, 1f)]
    public float DistanceToGround; // Distance from where the foot transform is to the lowest possible position of the foot.

    private void Start() {

        anim = GetComponent<Animator>();

    }

    private void OnAnimatorIK(int layerIndex) {

        if (anim) { // Only carry out the following code if there is an Animator set.

            // Set the weights of left and right feet to the current value defined by the curve in our animations.
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, anim.GetFloat(left_foot_ik_parameter));
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, anim.GetFloat(right_foot_ik_parameter));
            
           // float m_rp = anim.GetFloat(right_foot_ik_parameter) / 2;
           // float m_lp = anim.GetFloat(left_foot_ik_parameter) / 2;

           // anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, m_lp);
           // anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, m_rp);

            // Left Foot
            RaycastHit hit;
            // We cast our ray from above the foot in case the current terrain/floor is above the foot position.
            Ray ray = new Ray(anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out hit, DistanceToGround + 1f, layerMask)) {
                Vector3 footPosition = hit.point; // The target foot position is where the raycast hit a walkable object...
                footPosition.y += DistanceToGround; // ... taking account the distance to the ground we added above.
                anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
               // anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));

            }

            // Right Foot
            ray = new Ray(anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out hit, DistanceToGround + 1f, layerMask)) {
                Vector3 footPosition = hit.point;
                footPosition.y += DistanceToGround;
                anim.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                //anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hit.normal));
            }


        }

    }

}