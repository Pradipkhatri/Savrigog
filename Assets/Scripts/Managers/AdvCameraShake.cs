using UnityEngine;
using System.Collections;

namespace AdvCameraShake{
		public class Shakeitup{

		public static IEnumerator RotShake(Transform trans, float mag, float dur, float smoothing){

			Vector3 currentPosition = trans.localPosition;

			while(dur > 0){
				float noisex = Mathf.Cos(Time.time * smoothing) * mag;
				float noisey = Mathf.Sin(Time.time * smoothing) * mag;


				trans.localPosition = new Vector3(currentPosition.x + noisex, currentPosition.y + noisey, currentPosition.z);



				dur -= Time.deltaTime;
				yield return null;

			}

			trans.localPosition = currentPosition;
		}
	}
}