using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    Collider swordCollider;
    
    void Awake(){
        swordCollider = GetComponentInChildren<BoxCollider>();
    }

    void Update(){
        swordCollider.enabled = EnableCollider();
    }

    void DisableCollider(){
        swordCollider.enabled = false;
    }

    bool EnableCollider(){
        if(Game.control.player.movement.falling) return false;
        if(Game.control.player.pHealth.dead) return false;
        if(!Game.control.player.attack.attacking) return false;
        return true;
    }
}
