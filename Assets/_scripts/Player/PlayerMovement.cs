using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementProperties {

    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float sprintSpeed = 10f;
    public float turnSpeed = 15f;
    public float rollSpeed = 9f;
    
    public float runThreshold = 0.8f;

    public MovementProperties(){}
}

public class PlayerMovement : MonoBehaviour
{
    MovementProperties m;
    public Animator animator;
    public Transform cameraT;
    Vector3 moveDir;
    Vector2 input;

    float moveSpeed;
    bool walking;
    bool running;
    bool sprinting;
    bool rolling;

    bool rollButtonDown;
    float sprintTimer = 0;
    float sprintThreshold = 0.5f;


    void Awake(){
        m = new MovementProperties();
       cameraT = Camera.main.gameObject.transform;
    }

    void LateUpdate() {
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        DetermineSprint();
    }

    void FixedUpdate() {
        if(!rolling) Move(input);
        else ForcedRollMovement();
    }

    void Move(Vector2 input){
        moveDir = (cameraT.right*input.x) + (Vector3.Cross(cameraT.right, Vector3.up) * input.y).normalized;

        if(moveDir.magnitude != 0) { 
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), m.turnSpeed * Time.deltaTime);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        
        DetermineMoveSpeed(input);
    }

    void DetermineSprint(){
        if(!sprinting && Input.GetButton("Sprint")) sprintTimer += Time.deltaTime;
        
        if(sprintTimer < sprintThreshold && Input.GetButtonUp("Roll")) {
            sprintTimer = 0;
            StartRoll();
        }
        if(sprintTimer >= sprintThreshold) {
            sprinting = Input.GetButton("Sprint");
            if(!sprinting) sprintTimer = 0;
        }
    }
    
    //////////////////////////////////////////
    // ROLLING

    void StartRoll(){
        if(rolling) return;
        Debug.Log("roll");
        rolling = true;
        moveSpeed = m.rollSpeed;
        animator.SetTrigger("Roll");
    }
    
    void ForcedRollMovement(){
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    //Invoked from animation event
    public void MidRoll(){
        moveSpeed = m.rollSpeed / 2;
    }

    //Invoked from animationevent
    public void EndRoll(){
        Debug.Log("endroll");
        rolling = false;
    }

    void DetermineMoveSpeed(Vector2 input){
        float speed = input.magnitude;
        walking = speed > 0 && speed < m.runThreshold;
        running = speed >= m.runThreshold;

        if(walking) moveSpeed = m.walkSpeed;
        if(running) moveSpeed = m.runSpeed;
        if(sprinting) moveSpeed = m.sprintSpeed;
      
        animator.SetBool("Walking", walking);
        animator.SetBool("Running", running);
        animator.SetBool("Sprinting", sprinting);
    }
}
