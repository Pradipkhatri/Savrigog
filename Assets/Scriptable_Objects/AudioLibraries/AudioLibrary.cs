using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

 [Serializable]
public struct AudioArray{
    public string code;
    public AudioClip[] audioClip;
}

[Serializable]
public struct GroupAudioArray{
    public string code;
    public AudioArray[] audio_Array;
}
   

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Multiple Group Library")]
public class AudioLibrary : ScriptableObject
{
    [Serializable]
   public class GroupAudioContainer{
       public GroupAudioArray[] audioGroups;
   }

   public GroupAudioContainer group_audioContainer;
}

