using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleGrounder : MonoBehaviour
{
    // Start is called before the first frame update
   [SerializeField] Transform planeCollider;
    Transform player;
    [SerializeField] float rayCastHeight = 0.5f;
    [SerializeField] float max_rayLength = 5;
    [SerializeField] LayerMask groundLayers;
    [SerializeField] private Vector3 offset;

    void Start()
    {
        if(!planeCollider) planeCollider = this.gameObject.transform;
        player = PlayerManager.Instance.player.transform;
        offset = planeCollider.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        GroundChecker();
    }

    void GroundChecker(){
        if(!player) return;
        RaycastHit hit;

        Ray ray = new Ray (player.position + (Vector3.up * rayCastHeight), Vector3.down * max_rayLength);
        Debug.DrawRay(player.position + (Vector3.up * rayCastHeight), Vector3.down * max_rayLength, Color.cyan);

        if(Physics.Raycast(ray, out hit, max_rayLength, groundLayers)){
           //Vector3 newPosition = new Vector3(offset.x, hit.point.y, offset.z);
           //planeCollider.localPosition = newPosition + (Vector3.up * 0.1f);
           planeCollider.position = hit.point  + (Vector3.up * 0.1f);
        }
    }
}
