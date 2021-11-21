using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class BowMech_Line : MonoBehaviour
{
    [SerializeField] Transform start;
    [SerializeField] Transform middle;
    [SerializeField] Transform end;
    [SerializeField] LineRenderer line_renderer;

    [SerializeField] Transform middle_parent;

    [SerializeField] Vector3 arrow_look_offset;

    [Header("Bullet_Options")]
    public Transform arrow_object;
    [SerializeField] Transform arrow_to_bow_stick_point; 


    private void OnEnable(){
        Application.onBeforeRender += UpdatePostions;
    }

    private void OnDisable(){
        Application.onBeforeRender -= UpdatePostions;
    }

    void UpdatePostions(){
        Vector3[] positions = new Vector3[] {start.position, middle.position, end.position};
        line_renderer.SetPositions(positions);

        arrow_object.position = middle.position;
        arrow_object.rotation = Quaternion.LookRotation(arrow_to_bow_stick_point.position -  arrow_object.position) * Quaternion.Euler(arrow_look_offset);
    
        if(!PlayerManager.Instance.shootReady){
            middle.position = middle_parent.position;
        }else{
            middle.localPosition = Vector3.zero;
        }        
    }
}

