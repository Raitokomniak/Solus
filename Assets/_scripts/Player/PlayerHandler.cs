using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public Rigidbody rb;

    public Transform cameraT;
    public PlayerMovement movement;
    public PlayerAttack attack;
    public PlayerTargetingRange targetingRange;
    public PlayerTargeting pTarget;
    public PlayerHealth pHealth;

    public Animator animator;


    void Awake(){
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cameraT = Camera.main.gameObject.transform;
        pTarget = GetComponent<PlayerTargeting>();
        pHealth = GetComponent<PlayerHealth>();
    }

    public void TargetEnemy(bool toggle, Transform target){
        if(!toggle) pTarget.ReleaseTarget();
        else        pTarget.TargetEnemy(target);
    }

    public void ReleaseTarget(){
        
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
