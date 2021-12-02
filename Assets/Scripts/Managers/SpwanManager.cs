using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanManager : MonoBehaviour
{
    
    private static SpwanManager _instance;
    public static SpwanManager Instance{
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
        public GameObject prefab;
        public int size;
    }

    public List<Pool> objectPool;
    public Dictionary<string, Queue<GameObject>> objectDictionary;

    void Start(){
        objectDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach(Pool pool in objectPool){
            Queue<GameObject> op = new Queue<GameObject>();

            for(int i = 0; i < pool.size; i++){
                GameObject obj = Instantiate(pool.prefab, transform.position, transform.rotation);
                obj.SetActive(false);
                op.Enqueue(obj);
            }

            objectDictionary.Add(pool.tag, op);
        }
    }

    public GameObject SpwanFromPool (string tag, Vector3 position, Quaternion rotation, bool rePosition){
        
        if(!objectDictionary.ContainsKey(tag)){
            Debug.LogWarning("Object with tag: " + tag + " doesn't exist");
            return null;
        }

        GameObject obj = objectDictionary[tag].Dequeue();
        
        if(obj == null) return null;

        if(obj.activeInHierarchy){
            if(!rePosition){
                //Debug.LogWarning("Enemies Out Numbered!");
                return null;
            }
        }

        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        objectDictionary[tag].Enqueue(obj);

        return obj;
    }
}
