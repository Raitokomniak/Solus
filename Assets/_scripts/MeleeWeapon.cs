using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    Collider collider;
    bool colliderOn = true;
    
    void Awake(){
        collider = GetComponentInChildren<BoxCollider>();
    }

    void Update(){
        if(colliderOn && Game.control.player.pHealth.dead) DisableCollider();
    }

    void DisableCollider(){
        collider.enabled = false;
    }
}
