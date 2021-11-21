//There should be a cineMachine Impulse Source attached in GameManager.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    #region singleton
    private static GameManager _gameManager;

    public static GameManager gameManager{
        get { 
                //if(_gameManager == null){
                    //GameManager go = new GameObject().AddComponent<GameManager>();
                    //Instantiate(go);
                   // go.name = "GameManager";
                  //  _gameManager = go;
                //}
                return _gameManager; 
            }
    }

    void Awake()
    {
        if(!_gameManager){
            _gameManager = this;
            DontDestroyOnLoad(this);
        }else Destroy(this.gameObject);

       Initialize();
    }
    #endregion

    [HideInInspector] public Cinemachine.CinemachineImpulseSource impulseSource;

    [Tooltip("CurrentAim, MinAim, MaxAim")]
    public float aimSenstivity = 50;
    public bool invertX = false;
    public bool invertY = false;
    public bool aimAssist = false;
    public Image current_crossHair;
    public GameObject pauseMenu;
    public bool isPaused = false;

    [Header("Concrete, Wood, Metal, Blood")]
    public ParticleSystem[] swordContactParticles;
    public LayerMask wallsLayers;
    public LayerMask groundLayers;
    [SerializeField] MainMenu mainMenu;
    public List<GameObject> enemy_itemSpwans = new List<GameObject>();
    public AudioMixer master_mixer;

    public AudioSource music_Source;
    public AudioSource audio_Source;
    public AudioSource secoandary_audioSource;

    [System.Serializable]
    public struct cameras{
        public string cam_name;
        public int priority;
        public Cinemachine.CinemachineVirtualCamera actual_camera;

    }   
    public cameras[] cams;

    public int arrowCount;

    private void Initialize(){
        isPaused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        impulseSource = GetComponent<Cinemachine.CinemachineImpulseSource>();
    }
    void Update(){

        arrowCount = Mathf.Clamp(arrowCount, 0, 50);

        foreach(cameras c in cams){
            c.actual_camera.Priority = c.priority; 
        }
        
        if(Input.GetButtonDown("Pause") && !isPaused){
            mainMenu.PauseMenu();
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            isPaused = true;
        }
    }

    public void UnPause(){
        if(isPaused){
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            isPaused = false;
        }else return;
    }

    public void Senstivity(float value){
        aimSenstivity = value;
    }

    public void InvertX(bool value){
        invertX = value;
    }

    public void InvertY(bool value){
        invertY = value;
    }

    public void SaveGame(){
            SaveSystem.SavePlayer(PlayerManager.Instance);
    }

    public IEnumerator SlowMotionTrigger(float slowLength){        
        Time.timeScale = 0.05f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        master_mixer.SetFloat("MasterPitch", 0.7f);
        while(Time.timeScale < 1){
            Time.timeScale += (1f / slowLength) * Time.unscaledDeltaTime;
            if(Time.timeScale > 0.7f) master_mixer.SetFloat("MasterPitch", Time.timeScale);
            yield return null;
        }
        master_mixer.SetFloat("MasterPitch", 1f);
        Time.timeScale = 1;
    }
    public void LoadGame(){
            
            UnPause();
            
            PlayerData data = SaveSystem.LoadPlayer();

            Transform player =  PlayerManager.Instance.player.transform;

            PlayerManager.Instance.isDead = false;
            PlayerManager.Instance.playerHealth = data.max_playerHealth;
            PlayerManager.Instance.playerMaxHealth = data.max_playerHealth;
            PlayerManager.Instance.maxStaminaLevel = data.max_playerStamina;
            arrowCount = data.arrowCount;
            PlayerManager.Instance.scoreStore = data.kills;
            
            Vector3 position;
            position.x = data.playerPosition[0];
            position.y = data.playerPosition[1];
            position.z = data.playerPosition[2];

            player.transform.position = position;
            PlayerManager.Instance.anim.Play("OnGrounded");
    }

    public void ShakeCamera(float intensity, float time, Vector3 p){
        StopCoroutine("StartShakeCamera");
        StartCoroutine(StartShakeCamera(intensity, time, p));
    }

    private IEnumerator StartShakeCamera(float intensity, float time, Vector3 p){
        
        while(time > 0){
            impulseSource.GenerateImpulseAt(p, Vector3.one * intensity * time);
            time -= Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
}
