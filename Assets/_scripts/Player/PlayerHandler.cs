using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStats {
    public int maxStamina = 100;
}

public class PlayerHandler : MonoBehaviour
{
    public Rigidbody rb;
    public PlayerMovement movement;
    public PlayerAttack attack;
    public PlayerTargetingRange targetingRange;
    public PlayerTargeting pTarget;
    public PlayerHealth pHealth;

    public PlayerResources resources;

    public Animator animator;

    public Transform cameraT;
    
    public PlayerStats stats;
    

    public void Init(){
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cameraT = Camera.main.gameObject.transform;
        pTarget = GetComponent<PlayerTargeting>();
        pHealth = GetComponent<PlayerHealth>();
        resources = GetComponentInChildren<PlayerResources>();
        stats = new PlayerStats();

        movement.player = this;

        resources.Init();
    }

    public void TargetEnemy(bool toggle, Transform target){
        if(!toggle) pTarget.ReleaseTarget();
        else        pTarget.TargetEnemy(target);
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
