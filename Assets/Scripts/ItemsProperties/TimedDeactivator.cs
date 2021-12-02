using UnityEngine;

public class TimedDeactivator : MonoBehaviour
{
    [SerializeField] float timer;
    void OnEnable(){
        Invoke("Deactivate", timer);
    }

    void Deactivate(){
        this.gameObject.SetActive(false);
    }
}
