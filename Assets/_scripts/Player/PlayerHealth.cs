using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public bool dead;

    void LateUpdate(){
        if(Input.GetKeyDown(KeyCode.J)){
            Die();
        }
    }

    public void Die(){
        dead = true;
        Game.control.player.Animate("Die");
    }
}