using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Single Group Library")]
public class SingleGroupLibrary : ScriptableObject{

   [System.Serializable]
   public struct GroupAudioArray{
       public AudioArray[] audio_Array;
   }

   public GroupAudioArray audioContainer;
}

