using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpwan : MonoBehaviour {

	[System.Serializable]
	public struct WaveProperty{
		public string name;
		public Vector2Int[] enemyProperty;
	}

	[SerializeField] AudioClip wave_start_audio;
	[SerializeField] AudioClip war_start_audio;
	AudioSource music_Source;
	[SerializeField] Vector2 generateArea = new Vector2(3, 3);
	[SerializeField] GameObject[] toSpwan;
	
	[Header("Enemy_Num, Count")]
	[SerializeField] WaveProperty[] waveProperties;
	[SerializeField] GameObject barrier;

	GameObject trigger;
	int waves_remain;
	public void SpwannerTriggered(GameObject t){
		music_Source = GameManager.gameManager.music_Source;
		music_Source.clip = war_start_audio;
		music_Source.Play();
		trigger = t;
		waves_remain = waveProperties.Length;
		if(barrier != null) barrier.SetActive(true);
		
		StartCoroutine(StartWave());
	}

	public void CheckRemainingEnemies(){
		int remaining_enemies = transform.childCount - 1;
		if(remaining_enemies <= 2){ //1 is the barrier object, 2 is the last enemy. If <= 2 then start new wave.
			if(waves_remain > 0){
				StartCoroutine(StartWave());
			}else{
				if(remaining_enemies <= 1) WavesCompleted(); else return;
			}
		}else return;
	}

	

	void WavesCompleted(){
		music_Source.clip = null;
		music_Source.Stop();
		Destroy(trigger);
		Destroy(barrier);
		Destroy(this.gameObject, 5);
	}

	private IEnumerator StartWave(){
		AudioCaller.PlayOtherSound(wave_start_audio);
		
		int currentWave = waveProperties.Length - waves_remain;
		for(int i = 0; i < waveProperties[currentWave].enemyProperty.Length; i++){
			int enemy_num = waveProperties[currentWave].enemyProperty[i].x;
			for(int a = 0; a < waveProperties[currentWave].enemyProperty[i].y; a++) {
				SpwanEnemy(enemy_num);
				yield return new WaitForSeconds(2);
			}
		}
		waves_remain -= 1;
	}
		

	void SpwanEnemy(int enemy_num){
		float newPosX = transform.position.x + Random.Range(-generateArea.x / 2, generateArea.x / 2);
		float newPosZ = transform.position.z + Random.Range(-generateArea.y / 2, generateArea.y / 2);
		GameObject enemy = Instantiate(toSpwan[enemy_num], new Vector3(newPosX, transform.position.y, newPosZ), Quaternion.identity);
		enemy.transform.SetParent(this.gameObject.transform);
		return;
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, new Vector3(generateArea.x, 0, generateArea.y));
	}
}
