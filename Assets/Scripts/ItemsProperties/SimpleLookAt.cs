using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLookAt : MonoBehaviour
{
    private Transform target;

    private void Start()
    {
        target = PlayerManager.Instance.mainCamera.transform;
    }

    private void Update()
    {
       // Vector3 lookDir = target.position - transform.position;
      //  lookDir.y = 0;

        //transform.rotation = Quaternion.Lerp();
        transform.LookAt(target.position);
    }
}
