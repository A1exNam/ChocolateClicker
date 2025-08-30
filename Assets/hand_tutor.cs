using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hand_tutor : MonoBehaviour{
    Vector3 start_pos;
    void Start(){
        start_pos = transform.position;
    }
    void Update(){
        transform.position = start_pos + transform.up 
            * Mathf.PingPong(Time.time * consts.hand_tutor_speed, consts.hand_tutor_max_dist);   
    }
}
