using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingWall : MonoBehaviour, IClimbed<string>
{
    
    [SerializeField] LayerMask wallLayers;
    [SerializeField] private float wall_Height;
    [Tooltip("How far a ray can notice wall edge")]
    [SerializeField] private float wall_reach_max = 1.5f;
    [SerializeField] private int wall_reach_smoothing = 12;
    [SerializeField] private float wall_reach;
    private Vector3 current_normal;

    [SerializeField] private Vector3 hangPos;
    private Transform player;
    private Animator anim;
    [SerializeField] private float playerHeight = 1.7f;

    [SerializeField] private Vector3 rayLengths = new Vector3(5,5,5);
    Vector3[] ray_hitPos = new Vector3[4];
    public Vector3 landPos;
    public float land_forward_multiplyer = 0.2f;

    private bool grounded;
    public bool wall_Available;

    bool climb_process;

    private void Start(){
        player = PlayerManager.Instance.player.transform;
        anim = PlayerManager.Instance.anim;
    }

    public void ArtificialUpdate(){

         WallChecker();
        
        if(wall_Available)
            ShootRay();
        else
            PlayerActions.wall_found = false;

        if(!wall_Available) {
            for(int i = 0; i < 4; i++){
                ray_hitPos[i] = Vector3.zero;
            }
            PlayerActions.ledgeClimbing = false;
            wall_Height = 0;
        }
           
    }    
    private void WallChecker(){
            
        RaycastHit hit;
        Ray ray_groundChecker = new Ray(player.position,  -Vector3.up);
        Debug.DrawRay(player.position, -Vector3.up * rayLengths.x, Color.red);

        if(Physics.Raycast(ray_groundChecker, out hit, rayLengths.x)){
            ray_hitPos[0] = hit.point + Vector3.up * 0.2f;
            grounded = true;
        }else{
           ray_hitPos[0] = Vector3.zero; 
           grounded = false;
        }

        Ray ray_wallChecker;

        if(grounded){
            ray_wallChecker = new Ray(ray_hitPos[0], transform.forward);
            Debug.DrawRay(ray_hitPos[0], transform.forward * rayLengths.y, Color.red);
        }else{
            ray_wallChecker = new Ray(player.position, transform.forward);
            Debug.DrawRay(player.position, transform.forward * rayLengths.y, Color.red);
        }


        if(Physics.Raycast(ray_wallChecker, out hit, rayLengths.y, wallLayers)){
            ray_hitPos[1] = hit.point - transform.forward * 0.1f;
            wall_Available = true;
            current_normal = hit.normal;
        }else{
            ray_hitPos[1] = Vector3.zero;
            wall_Available = false;
            current_normal = Vector3.zero;
        }

    }

    private void ShootRay(){
        PlayerActions.wall_found = true;
        RaycastHit hit;

        bool minimize_Height;
        bool maximized_Height;

        Ray ray_availability = new Ray(ray_hitPos[1] + ray_hitPos[2], transform.forward * rayLengths.z);
        Debug.DrawRay(ray_hitPos[1] + ray_hitPos[2], transform.forward * rayLengths.z, Color.red);

        if(Physics.Raycast(ray_availability, out hit, rayLengths.z, wallLayers)){
            ray_hitPos[2] += new Vector3(0, 0.5f, 0);
            maximized_Height = false;
        }else{

            minimize_Height = true;
            maximized_Height = true;
        }

        ray_hitPos[3] = ray_hitPos[1] + ray_hitPos[2];

        Ray level_checker = new Ray(ray_hitPos[3] + transform.forward * rayLengths.z, -Vector3.up * rayLengths.z);
        Debug.DrawRay(ray_hitPos[3] + transform.forward * rayLengths.z, -Vector3.up * rayLengths.z, Color.green);

        if(Physics.Raycast(level_checker, out hit, rayLengths.z)){
            landPos = hit.point;
            minimize_Height = false;
        }  
        else minimize_Height = true;

        if(minimize_Height && maximized_Height) ray_hitPos[2] -= new Vector3(0, 0.5f, 0);


        Debug.DrawLine(ray_hitPos[1], ray_hitPos[3], Color.red);

        wall_Height = Vector3.Distance(ray_hitPos[1], ray_hitPos[3]);
        
        Vector3 player_grab_pos = new Vector3(player.position.x, player.position.y + playerHeight, player.position.z);
        Debug.DrawLine(player_grab_pos, landPos);
        
        wall_reach = Vector3.Distance(landPos, player_grab_pos);


        if(wall_reach < wall_reach_max && PlayerManager.Instance.cm.velocityY < 0 && PlayerManager.Instance.cm.airTime < 1.2f){
            
            if(PlayerActions.isGrounded) return;
            if(!climb_process) ClimbWall();
        }else return;
    }
    
    private void ClimbWall(){
            anim.SetBool("OnWall", true);
            PlayerActions.ledgeClimbing = true;

            player.rotation = Quaternion.LookRotation(-current_normal);
            Vector3 mainPos = new Vector3(landPos.x, landPos.y - playerHeight, landPos.z);
            Vector3 hang_Pos = mainPos + (current_normal * hangPos.z);
            Vector3 finalPos = hang_Pos + new Vector3(hangPos.x, hangPos.y, 0);
            StartCoroutine(GrabLedge(finalPos, 0.5f));

            AudioCaller.SingleGroupAudioPlay(null, GameManager.gameManager.secoandary_audioSource, "GrabLedge");
            GameManager.gameManager.impulseSource.GenerateImpulseAt(transform.position, Vector3.one);
            climb_process = true;
    }

    private IEnumerator GrabLedge(Vector3 finalPos, float timer){
        while(timer > 0){
            player.position = Vector3.Lerp(player.position, finalPos, wall_reach_smoothing * Time.deltaTime);
            timer -= Time.deltaTime;
            yield return null;
        }
        player.position = finalPos;
    }

    public void Climbed(string type)
	{
		if(type != "ClimbingWall") return;

        PlayerActions.isGrounded = true;
        Vector3 modified_landPos = landPos + transform.forward * land_forward_multiplyer;
        StartCoroutine(ClimbFinalizer(modified_landPos, 0.2f));

	}

    IEnumerator ClimbFinalizer(Vector3 pos, float time){
        while(time > 0){
            player.position = pos;
            time -= Time.deltaTime;
            yield return null;
        }
        climb_process = false;
        anim.SetBool("OnWall", false);
		PlayerActions.ledgeClimbing = false;
        yield return null;
    }
}