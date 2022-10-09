using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public PlayerMovement movement;
    public PlayerTargetingRange targetingRange;

    public bool EnemiesInRange(){
        if(targetingRange.enemiesInRange.Count > 0) return true;
        return false;
    }
}
