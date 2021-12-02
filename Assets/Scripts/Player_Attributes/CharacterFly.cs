using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFly : MonoBehaviour
{
    
    bool isflying = false;
    CharacterMovement cm;
    CharacterController cc;
    [SerializeField] float moveSpeed = 10;

    void Start(){
         cm = GetComponent<CharacterMovement>();
         cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U)){
            if(isflying){
                isflying = false;
                cc.enabled = true;
                cm.flying = false;
                cm.velocityY = 0f;
            }else{
                isflying = true;
                cm.flying = true;
            }
        }

        if(isflying) Flying(); 
    }

    void Flying(){
        cc.enabled = false;
        cm.airTime = 0;
        float speed = 0;
        bool boost = Input.GetButton("Boost");
        if(boost) speed = moveSpeed * 2; else speed = moveSpeed; 


        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        Vector3 forward = PlayerManager.Instance.mainCamera.forward;
        Vector3 right = PlayerManager.Instance.mainCamera.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();
        
        Vector3 direction = (forward * moveInput.y + right * moveInput.x).normalized;


        if(Input.GetKey(KeyCode.Space)){
            direction.y += Time.deltaTime * moveSpeed;
        }else if(Input.GetKey(KeyCode.Mouse0)){
            direction.y -= Time.deltaTime * moveSpeed;
        }else{
            direction.y = 0;
        }
        
        Vector3 dir = direction;
        dir.y = 0;

        if(dir != Vector3.zero) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * speed);

        transform.position = Vector3.Lerp(transform.position, transform.position + direction, Time.deltaTime * speed);
    }
    
}
