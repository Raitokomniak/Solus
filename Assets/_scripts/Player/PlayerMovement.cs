using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A small class to contain player properties
public class MovementProperties {

    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float sprintSpeed = 10f;
    public float turnSpeed = 15f;
    public float rollSpeed = 9f;
    public float backstepSpeed = 6f;
    
    public float runThreshold = 0.1f;
    public float sprintThreshold = 0.5f;

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
    bool moving;
   // bool walking;
    bool running;
    bool sprinting;
    bool rolling;
    bool backstepping;
    bool backstepMoving;

    float sprintTimer = 0;


    void Awake(){
        m = new MovementProperties();
       cameraT = Camera.main.gameObject.transform;
    }

    void LateUpdate() {
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moving = input.magnitude > 0;
        if(!moving && !rolling && Input.GetButtonDown("Roll")) BackStep();
        DetermineSprint();
        
    }

    void FixedUpdate() {
        if      (rolling)      ForcedRollMovement();
        else if (backstepping && backstepMoving) ForcedBackStepMovement();
        else Move();
    }

    void Move(){
        moveDir = (cameraT.right*input.x) + (Vector3.Cross(cameraT.right, Vector3.up) * input.y).normalized;

        if(moveDir.magnitude != 0) { 
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), m.turnSpeed * Time.deltaTime);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        
        DetermineMoveSpeed(input);
    }

    void DetermineSprint(){
        if(!moving) return;
        if(!sprinting && Input.GetButton("Sprint")) sprintTimer += Time.deltaTime;
        
        if(sprintTimer < m.sprintThreshold && Input.GetButtonUp("Roll")) {
            sprintTimer = 0;
            StartRoll();
        }
        if(sprintTimer >= m.sprintThreshold) {
            sprinting = Input.GetButton("Sprint");
            if(!sprinting) sprintTimer = 0;
        }
    }

    //////////////////////////////////////////
    // Backstepping

    void BackStep(){
        if(backstepping) return;
        backstepping = true;
        moveSpeed = m.backstepSpeed;
        animator.SetTrigger("Backstep");
    }

    void StartBackStepMovement(){
        backstepMoving = true;
    }

    //Invoked from animation event
    //CHANGE SPEED INTO A CURVE
    void MidBackStep(){
        moveSpeed = 0;
    }

    void EndBackStep(){
        backstepping = false;
        backstepMoving = false;
    }
    
    void ForcedBackStepMovement(){
        transform.position -= transform.forward * moveSpeed * Time.deltaTime;
    }

    //////////////////////////////////////////
    // ROLLING

    void StartRoll(){
        if(rolling) return;
        rolling = true;
        moveSpeed = m.rollSpeed;
        animator.SetTrigger("Roll");
    }
    
    void ForcedRollMovement(){
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
    

    //Invoked from animation event
    //CHANGE SPEED INTO A CURVE
    public void MidRoll(){
        moveSpeed = m.rollSpeed / 2;
    }

    //Invoked from animationevent
    //CHANGE SPEED INTO A CURVE
    public void EndRoll(){
        rolling = false;
    }

    void DetermineMoveSpeed(Vector2 input){
        float speed = input.magnitude;
   //     walking = speed > 0 && speed < m.runThreshold;
        running = speed >= m.runThreshold;

     //   if(walking) moveSpeed = m.walkSpeed;
        if(running) moveSpeed = m.runSpeed;
        if(sprinting) moveSpeed = m.sprintSpeed;
      
     //   animator.SetBool("Walking", walking);
        animator.SetBool("Running", running);
        animator.SetBool("Sprinting", sprinting);
    }
}
