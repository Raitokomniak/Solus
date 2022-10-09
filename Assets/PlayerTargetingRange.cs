using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetingRange : MonoBehaviour
{
    public List<Collider> enemiesInRange;

    void Awake(){
        enemiesInRange = new List<Collider>();
    }

    void Update(){
       
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Enemy"){
            enemiesInRange.Add(other);
        }    
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Enemy"){
            enemiesInRange.Remove(other);
            if(enemiesInRange.Count == 0) Game.control.cam.ReleaseTarget();
        }    
    }

    //change type
    public Transform GetClosestEnemy(){
        return enemiesInRange[0].transform;
    }
}
