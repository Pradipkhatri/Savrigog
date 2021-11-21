using UnityEngine;

public static class AudioCaller
{
    public static AudioSource master_source;
    public static void PlayOtherSound(AudioClip audio){
        if(master_source == null) master_source = GameObject.Find("MasterSource").GetComponent<AudioSource>();
        //master_source = GameManager.gameManager.audio_Source;
        master_source.PlayOneShot(audio);
    }

    public static void SingleGroupAudioPlay(SingleGroupLibrary sgl, AudioSource audioSource, string code){
        try{
            if(sgl == null) sgl = PlayerManager.Instance.playerAudios;
            foreach(AudioArray aa in sgl.audioContainer.audio_Array){
                if(aa.code == code){
                    if(audioSource != null) 
                        PlaySound(audioSource, ChooseAudio(aa));
                    else
                        PlayOtherSound(ChooseAudio(aa));

                    return;
                }
            }
            Debug.Log("No Audio Match Found :: " + code + " :: Single Group Audio!");
            return;
            }
        catch{
            return;
        }
        
    }

    public static void MultiGroupAudioPlay(AudioLibrary al, AudioSource audioSource, string parent, string code){
        if(al == null) al = PlayerManager.Instance.mainAudios;
        GroupAudioArray groupAudioArray = new GroupAudioArray();
        
        try{
            foreach(GroupAudioArray gaa in al.group_audioContainer.audioGroups){
                if(gaa.code == parent){
                    groupAudioArray = gaa;
                    break;
                }
            }
        }
        catch{
            Debug.Log("No Parent Match Found :: " + parent + " :: Please correct the name!");
            return;
        }

        try{
            foreach(AudioArray aa in groupAudioArray.audio_Array){
                if(aa.code == code){
                    if(audioSource == null) PlayOtherSound(ChooseAudio(aa)); else PlaySound(audioSource, ChooseAudio(aa));
                    return;
                }
            }
        }
        catch{
            Debug.Log("No Audio Match Found :: " + code + " :: Please correct the name!");
            return;
        }

        Debug.Log("No Audio Match Found :: " + parent + " :: "+ code + " :: Please correct the name!");
        return; 
      

    }

    private static AudioClip ChooseAudio(AudioArray aa){
        int a = Random.Range(0, aa.audioClip.Length);
        return aa.audioClip[a];
    }

    public static void PlaySound(AudioSource audioSource, AudioClip audio){
        audioSource.PlayOneShot(audio);
    }
}
