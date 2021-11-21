using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{

    [SerializeField] GameObject selection_MainMenu, selection_PauseMenu, selection_OptionMenu, selection_AudioMenu, selection_VideoMenu, selection_ControlsMenu;

    [SerializeField] string currentMenu;

    [SerializeField] AudioMixer audioMixer;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if(EventSystem.current.currentSelectedGameObject == null){
            switch(currentMenu){
                case "MainMenu":
                    Menu();
                    break;
                case "PauseMenu":
                    PauseMenu();
                    break;
                case "OptionMenu":
                    OptionMenu();
                    break;
                case "AudioMenu":
                    AudioMenu();
                    break;
                case "VideoMenu":
                    VideoMenu();
                    break;
                case "ControlsMenu":
                    ControlsMenu();
                    break;
                default:
                    break;
            }
        }
    }   
    
    public void GameVolume(float value){
            audioMixer.SetFloat("GameVolume", Mathf.Log10(value) * 20);
    }

    public void MusicVolume (float value){
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }

    public void VoiceVolume (float value){
        audioMixer.SetFloat("VoiceVolume", Mathf.Log10(value) * 20);
    }

    public void QualitySetting(int qualityIndex){
            QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen){
            Screen.fullScreen = isFullScreen;
    }
    
    public void NewGame(){
        SceneManager.LoadScene(1);
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void ReturnMainMenu(){
        SceneManager.LoadScene(0);
        currentMenu = "MainMenu";
    }

    public void PauseMenu(){
        currentMenu = "PauseMenu";
        if(selection_PauseMenu != null){
            StartCoroutine(Delay(selection_PauseMenu));
        }else return;
    }

    public void OptionMenu(){
        currentMenu = "OptionMenu";
        if(selection_OptionMenu != null){
        StartCoroutine(Delay(selection_OptionMenu));
        }else return;
    }

    public void Menu(){
        currentMenu = "MainMenu";
        if(selection_MainMenu != null){
            StartCoroutine(Delay(selection_MainMenu));
        }else return;
    }

    public void AudioMenu(){
        currentMenu = "AudioMenu";
        if(selection_AudioMenu != null){
            StartCoroutine(Delay(selection_AudioMenu));
        }else return;
    }

    public void VideoMenu(){
        currentMenu = "VideoMenu";
        if(selection_VideoMenu != null){
            StartCoroutine(Delay(selection_VideoMenu));
        }else return;
    }

    public void ControlsMenu(){
        currentMenu = "ControlsMenu";
        if(selection_ControlsMenu != null){
            StartCoroutine(Delay(selection_ControlsMenu));
        }else return;
    }


    private IEnumerator Delay(GameObject menu){
        EventSystem.current.SetSelectedGameObject(null);
        //yield return new WaitForSeconds(0.1f);
        yield return null;
        EventSystem.current.SetSelectedGameObject(menu);

    }
}
