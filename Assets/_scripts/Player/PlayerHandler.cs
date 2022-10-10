using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public Rigidbody rb;

    public PlayerMovement movement;
    public PlayerAttack attack;
    public PlayerTargetingRange targetingRange;

    public Animator animator;


    void Awake(){
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    public bool EnemiesInRange(){
        if(targetingRange.enemiesInRange.Count > 0) return true;
        return false;
    }
    
    public void Animate(string key, bool value){
        animator.SetBool(key, value);
    }

    public void Animate(string key){
        animator.SetTrigger(key);
    }
}