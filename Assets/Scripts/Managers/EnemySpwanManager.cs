using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpwanManager : MonoBehaviour
{
    
    private static EnemySpwanManager _instance;
    public static EnemySpwanManager Instance{
        get{
            return _instance;
        }
    }

    void Awake(){
        if(_instance == null) _instance = this; else Destroy(this.gameObject);
    }
    
    [System.Serializable]
    public struct Pool{
        public string tag;
        public GameObject enemyPrefab;
        public int size;
    }

    public List<Pool> enemyPool;
    public Dictionary<string, Queue<GameObject>> enemyDictionary;

    void Start(){
        enemyDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach(Pool pool in enemyPool){
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for(int i = 0; i < pool.size; i++){
                GameObject obj = Instantiate(pool.enemyPrefab, transform.position, transform.rotation);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            enemyDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpwanFromPool (string tag, Vector3 position, Quaternion rotation){
        
        if(!enemyDictionary.ContainsKey(tag)){
            Debug.LogWarning("Enemy with tag: " + tag + " doesn't exist");
            return null;
        }

        GameObject enemy = enemyDictionary[tag].Dequeue();

        if(enemy.activeInHierarchy){
            Debug.LogWarning("Enemies Out Numbered!");
            return null;
        }

        enemy.SetActive(true);
        enemy.transform.position = position;
        enemy.transform.rotation = rotation;

        enemyDictionary[tag].Enqueue(enemy);

        return enemy;
    }
}
