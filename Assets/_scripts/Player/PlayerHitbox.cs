using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    public string hitBoxType;

    void Awake(){

    }

    void Update(){
        GetComponent<Collider>().enabled = EnableCollider();
    }

    bool EnableCollider(){
        if(Game.control.player.movement.falling) return false;
        if(Game.control.player.movement.rolling) return false;
       // if(Game.control.player.movement.backstepping) return false;
        return true;
    }
}
