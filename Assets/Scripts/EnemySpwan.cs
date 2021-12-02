using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpwan : MonoBehaviour {

	[System.Serializable]
	public struct WaveProperty{
		public string name;
		public Vector2Int[] enemyProperty;
	}

	private SpwanManager spwanManager;

	[SerializeField] AudioClip wave_start_audio;
	[SerializeField] AudioClip war_start_audio;
	AudioSource music_Source;
	[SerializeField] Vector2 generateArea = new Vector2(3, 3);
	[SerializeField] string[] enemyNames;
	
	[Header("Enemy_Num, Count")]
	[SerializeField] WaveProperty[] waveProperties;
	[SerializeField] GameObject barrier;

	int left_to_spwan;
	GameObject trigger;
	int waves_remain;
	public void SpwannerTriggered(GameObject t){
		spwanManager = SpwanManager.Instance;
		music_Source = GameManager.gameManager.music_Source;
		music_Source.clip = war_start_audio;
		music_Source.Play();
		trigger = t;
		waves_remain = waveProperties.Length;
		if(barrier != null) barrier.SetActive(true);
		
		StartCoroutine(StartWave());
	}

	public void CheckRemainingEnemies(){
		if(left_to_spwan > 0) return;
		int remaining_enemies = transform.childCount - 1;
		if(remaining_enemies <= 2){ //1 is the barrier object, 2 is the last enemy. If <= 2 then start new wave.
			if(waves_remain > 0){
				StartCoroutine(StartWave());
			}else{
				if(remaining_enemies <= 1) WavesCompleted(); else return;
			}
		}else return;
	}

	void Recheck(){
		CheckRemainingEnemies();
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
		waves_remain -= 1;

		WaveProperty currentProp = waveProperties[currentWave];

		for(int j = 0; j < currentProp.enemyProperty.Length; j++){
			left_to_spwan += currentProp.enemyProperty[j].y;
		}
		
		for(int i = 0; i < currentProp.enemyProperty.Length; i++){
			int enemy_num = currentProp.enemyProperty[i].x;
			
			for(int a = 0; a < currentProp.enemyProperty[i].y; a++) {
				string choosenEnemy = enemyNames[enemy_num];
				SpwanEnemy(choosenEnemy);
				left_to_spwan--;
				yield return new WaitForSeconds(2);
			}
		}
		
	}
		

	void SpwanEnemy(string enemyName){
		float newPosX = transform.position.x + Random.Range(-generateArea.x / 2, generateArea.x / 2);
		float newPosZ = transform.position.z + Random.Range(-generateArea.y / 2, generateArea.y / 2);
		GameObject enemy = spwanManager.SpwanFromPool(enemyName, new Vector3(newPosX, transform.position.y, newPosZ), Quaternion.identity, false);
		if(enemy != null) enemy.transform.SetParent(this.gameObject.transform); else CheckRemainingEnemies();
		return;
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, new Vector3(generateArea.x, 0, generateArea.y));
	}
}
