using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    Collider swordCollider;
    bool colliderOn = true;
    
    void Awake(){
        swordCollider = GetComponentInChildren<BoxCollider>();
    }

    void Update(){
        if(colliderOn && Game.control.player.pHealth.dead) DisableCollider();
    }

    void DisableCollider(){
        swordCollider.enabled = false;
    }
}
