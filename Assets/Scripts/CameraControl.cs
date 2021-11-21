using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    
    [SerializeField] Vector2 yClamp = new Vector2(-90, 90);
    //[SerializeField] float aimSenstivity;

    float rotX;
    float rotY;

    [SerializeField] Transform target;
    [SerializeField] Vector3 loc_offset = new Vector3(0, 1.3f, 0);
    [SerializeField] float follow_smoothing = 5;
    [SerializeField] int rotate_Smoothing = 10;

    Vector3 velocity;
    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotX = rot.x;
        rotY = rot.y;
    }

    void FixedUpdate()
    {
        Vector3 desirePos = target.transform.position + loc_offset;

        transform.position = Vector3.SmoothDamp(transform.position, desirePos, ref velocity, follow_smoothing * Time.deltaTime);

        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        if(!GameManager.gameManager.invertX)
            rotX += x * GameManager.gameManager.aimSenstivity * Time.deltaTime;
        else
            rotX -= x * GameManager.gameManager.aimSenstivity * Time.deltaTime;
        
        if(!GameManager.gameManager.invertY)
            rotY -= y * GameManager.gameManager.aimSenstivity * Time.deltaTime;
        else
            rotY += y * GameManager.gameManager.aimSenstivity * Time.deltaTime;

        if(!PlayerActions.objectPushing) 
            rotY = Mathf.Clamp(rotY, yClamp.x, yClamp.y);
        else
            rotY = Mathf.Clamp(rotY, -40, 40);

        Quaternion localRotation = Quaternion.Euler(rotY, rotX, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, localRotation, Time.deltaTime * rotate_Smoothing);
    }
}
