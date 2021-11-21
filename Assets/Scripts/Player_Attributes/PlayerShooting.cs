using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HumanBone{
    public HumanBodyBones bone;
    public float weight;
}
public class PlayerShooting : MonoBehaviour
{

    PlayerAttack pa;
    Animator anim;
    float current_bow_charge;
    bool trigged = false; //Bow Trigger
    float recover_speed;

    [SerializeField] private List<BowMech> all_shooting_weapons = new List<BowMech>();
    [SerializeField] Transform camTarget;
    private WeaponProperties current_weaponProperty;
    private int current_weaponID;
    [HideInInspector] public BowMech current_bow_properties;
    private Transform player;
    private Transform mainCamera;
    [SerializeField] ArrowInventory arrow_Inventory;


    [Space(10)]
    [Header("Aiming Properties")]
    [SerializeField] float aim_speed = 10;
    [SerializeField] [Range(0, 1)] float look_at_weight = 1;
    [SerializeField] HumanBone[] humanBones;
    Transform[] boneTransforms;
    [SerializeField] float angle_limit = 90;
    [SerializeField] float distance_Limit = 1.5f;
    [SerializeField] Transform targetObject;
    [SerializeField] Vector3 lookatOffset;
    [SerializeField] Vector3 lookatOffset_body;
    [SerializeField] float target_distance = 5;
    bool enableTargeting;

    [Space(10)]
    [Header("ShootProperty")]
    [SerializeField] UnityEngine.UI.Text ammo_UI;
    UnityEngine.UI.Image crossHair;
    [SerializeField] LayerMask arrowCollisions;
    
    [Space(10)]
    [Header("Audios")]
    [SerializeField] AudioClip arrow_type_changeAudio;
    [SerializeField] AudioClip no_ammo_audio;

    int bulletCount;
    void Start()
    {
        anim = PlayerManager.Instance.anim;
        pa = GetComponent<PlayerAttack>();
        crossHair = GameManager.gameManager.current_crossHair;
        player = PlayerManager.Instance.player.transform;
        mainCamera = PlayerManager.Instance.mainCamera;
        current_weaponID = 0;
        boneTransforms = new Transform[humanBones.Length];
        for(int i = 0; i < boneTransforms.Length; i++){
            boneTransforms[i] = anim.GetBoneTransform(humanBones[i].bone);
        }
    }

    // Update is called once per frame

    void OnEnable(){
        WeaponSelector();
    }

    void WeaponSelector(){

        int total_Weapons = all_shooting_weapons.Count;

        if(total_Weapons > 1){
            if(current_weaponID < (all_shooting_weapons.Count - 1)){
                current_weaponID += 1;
            }else{
                current_weaponID = 0;
            }
        }
        
        current_bow_properties = all_shooting_weapons[current_weaponID];

        if(all_shooting_weapons.Count > 1){
            foreach (BowMech bm in all_shooting_weapons) {
                if (current_weaponID != bm.hand_object.GetComponent<WeaponProperties> ().weaponID) {
                    bm.hand_object.SetActive (false);
                    bm.show_object.SetActive (false);
                }
            }
        }

        current_weaponProperty = current_bow_properties.gameObject.GetComponent<WeaponProperties>();
    }

    void LateUpdate()
    {        
        
        if(PlayerManager.Instance.isDead || GameManager.gameManager.isPaused) return;

        if(Input.GetKeyDown(KeyCode.E) && !PlayerActions.Busy){
            AudioCaller.SingleGroupAudioPlay(null, null, "WeaponChange");
            WeaponSelector();
        }

        WeaponUpdate();

        bulletCount = arrow_Inventory.arrow_collection[current_weaponID].inventory;
        
        ammo_UI.text = "" + bulletCount;
        
        if(PlayerActions.isGrounded){
            AutoTarget();

            Vector3 dir = targetObject.position - transform.position;
            Vector3 target_position = GetTargetPosition(dir);
            
            if(enableTargeting){
                BowAim();
            }else{
                if(PlayerActions.NotPerforming() && !PlayerActions.RootMotionHandler()){
                    for(int i = 0; i < 3; i++){
                        for(int j = 0; j < boneTransforms.Length; j++){
                            Transform bone = boneTransforms[j];
                            float boneWeight = humanBones[j].weight * look_at_weight;
                            AimFunction(bone, target_position, boneWeight);
                        }
                    }
                }
            }
        } 

       
        #region TemporaryHide
        if(Input.GetButtonDown("Fire1") && bulletCount <= 0){
            AudioCaller.PlayOtherSound(no_ammo_audio);
        }

        if(!PlayerActions.isAttacking && PlayerActions.NotPerforming() && !PlayerManager.Instance.isDead && !PlayerActions.ledgeWalking) 
        {
            if(Input.GetButton("Aim"))
            {
                if(bulletCount > 0) 
                    Aim();
                else{
                    UnAim();
                }
            }else UnAim();
        }else UnAim();

        #endregion
    }

    void BowAim(){
        Vector3 new_dir = mainCamera.forward * target_distance;
        BowAimFunction(boneTransforms[0], new_dir, 1);
    }

    void WeaponUpdate(){
        if(current_bow_properties == null) return;
         if (PlayerActions.isAiming) {
            current_bow_properties.hand_object.SetActive(true);
            current_bow_properties.show_object.SetActive(false);
		} else {
			if (anim.GetFloat ("MovementSpeed") > 1.3f || PlayerActions.objectPushing || PlayerActions.isGrounded) {
                current_bow_properties.hand_object.SetActive(false);
                current_bow_properties.show_object.SetActive(true);
			}
		}
    }

    private void BowAimFunction(Transform bone, Vector3 dir, float weight){
        //dir.y = 0;
        Vector3 rotation = mainCamera.forward;
        rotation.y = 0;
        Quaternion final_rot = Quaternion.LookRotation(rotation, Vector3.up) * Quaternion.Euler(lookatOffset_body);
        //player.rotation = final_rot;
        player.rotation = Quaternion.Lerp(transform.rotation, final_rot, aim_speed * Time.deltaTime);
        
        
        if(bone != null) {
            Quaternion new_rot = Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(lookatOffset);
            bone.rotation = new_rot;
        }   
    }
    
    void AimFunction(Transform bone, Vector3 target_position, float weight){
        if(PlayerActions.isAiming) return;
        target_position.y = transform.position.y - 0.1f;
        Vector3 aimDirection = PlayerManager.Instance.cc.transform.forward;
        Vector3 targetDirection = target_position - PlayerManager.Instance.cc.transform.position;
        Quaternion new_rot = Quaternion.FromToRotation(aimDirection, targetDirection);
        Quaternion blend_rot = Quaternion.Slerp(Quaternion.identity, new_rot, weight);
        bone.rotation = blend_rot * bone.rotation;
    }


    Vector3 GetTargetPosition(Vector3 dir){
        float blend_out = 0.0f;

        float target_angle = Vector3.Angle(dir, transform.forward);
        
        if(target_angle > angle_limit){
            blend_out += (target_angle - angle_limit) / 50;
        }

        float targetDistance = dir.magnitude;
        if(targetDistance < distance_Limit){
            blend_out += distance_Limit - targetDistance;
        }

        Vector3 direction = Vector3.Slerp(dir, transform.forward, blend_out);
        return transform.position + direction;
    }

    void AutoTarget(){ 
        
        Vector3 new_targetPos = camTarget.position + camTarget.forward * target_distance;

        if(pa.target_enemy != null){
            new_targetPos = pa.target_enemy.position;
        }

        targetObject.position = Vector3.Slerp(targetObject.position, new_targetPos, Time.deltaTime * 8);
    }

    void UnAim(){
        crossHair.enabled = false;
        GameManager.gameManager.cams[1].priority = 5;
        enableTargeting = false;
        anim.SetLayerWeight(anim.GetLayerIndex("ShootingArrow"), Mathf.Lerp(anim.GetLayerWeight(2), 0, Time.deltaTime * 8));
        PlayerActions.isAiming = false;
        anim.SetBool("Aiming", false);
    }

    void Aim(){
        
        crossHair.enabled = true;
        GameManager.gameManager.cams[1].priority = 15;
        anim.SetLayerWeight(anim.GetLayerIndex("ShootingArrow"), 1);
        enableTargeting = true;
        
        if(recover_speed > 0) recover_speed -= Time.deltaTime;

        PlayerActions.isAiming = true;
        anim.SetBool("Aiming", true);


       current_bow_properties.arrow_object.SetActive(!PlayerManager.Instance.shootReady);

        if(Input.GetButton("Fire1")) {
            if(!trigged){
                ResetCharge();
                trigged = true;
            }
            ChargeBow();
        }

        if(Input.GetButtonUp("Fire1") && trigged){
            if(recover_speed <= 0){
                BowShoot();
                trigged = false;
                recover_speed = current_bow_properties.max_recover_speed;
            }
        }
    }

    void ChargeBow(){
        current_bow_charge += Time.deltaTime * current_bow_properties.charge_speed;
        current_bow_properties.current_charge_state = Mathf.Clamp(Mathf.FloorToInt(current_bow_charge), 1, 3);
        anim.SetFloat("BowCharge", current_bow_properties.current_charge_state , 0.1f, Time.deltaTime);
    }

    void ResetCharge(){
        current_bow_properties.current_charge_state = 1;
        current_bow_charge = 0;
        anim.SetFloat("BowCharge", 0);
    }

    void BowShoot(){

        current_bow_properties.arrow_object.SetActive(false);
        
        AudioCaller.PlayOtherSound(current_bow_properties._shootAudio);

        if(bulletCount > 0) arrow_Inventory.arrow_collection[current_weaponID].inventory -= 1;
        
        GameManager.gameManager.impulseSource.GenerateImpulseAt(transform.position, Vector3.one);
        anim.SetTrigger("Shoot");

        GameObject parent_arrow = Instantiate(arrow_Inventory.arrow_collection[0].parent_arrow_object, mainCamera.position + (mainCamera.forward * 3), mainCamera.rotation);
        GameObject child_arrow = Instantiate(arrow_Inventory.arrow_collection[0].child_arrow_object, current_bow_properties.spwan_loc.position, current_bow_properties.spwan_loc.rotation);

        ParentArrow pa = parent_arrow.GetComponent<ParentArrow>();
        pa.Initialize();
        ChildArrow ca = child_arrow.GetComponent<ChildArrow>();
            
        pa.parent_arrow_properties.child_arrow = ca.child_arrow_properties;
        pa.parent_arrow_properties.child_arrow_object = child_arrow.transform;
        ResetCharge();
    }

    void BowStretch(){
        
        AudioCaller.PlayOtherSound(current_bow_properties._stretchAudio);
    }
    
}
